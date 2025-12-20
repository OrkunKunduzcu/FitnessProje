using FitnessProje.Web.Data;
using FitnessProje.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessProje.Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Üyenin Kendi Randevularını Listelemesi
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            var appointments = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Where(a => a.AppUserId == user.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        // 2. Randevu Alma Sayfası (GET)
        public IActionResult Create()
        {
            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View();
        }

        // 3. Randevuyu Kaydetme (POST) - GELİŞMİŞ KONTROL
        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Kullanıcıdan gelmeyen alanları modelden düş
            ModelState.Remove("AppUser");
            ModelState.Remove("Trainer");
            ModelState.Remove("Service");
            ModelState.Remove("AppUserId");

            // --- 1. HİZMET VE SÜRE BİLGİSİNİ ÇEK ---
            // Randevu detayları (Süre, Ücret vb.) Service tablosunda saklanıyor.
            var selectedService = await _context.Services.FindAsync(appointment.ServiceId);
            if (selectedService == null)
            {
                ModelState.AddModelError("", "Lütfen geçerli bir hizmet seçiniz.");
            }

            // --- 2. SAAT VE TARİH KONTROLLERİ ---
            
            // A) Geçmiş Tarih Kontrolü
            if (appointment.AppointmentDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarihe randevu alamazsınız.");
            }

            // B) Salon Çalışma Saatleri (09:00 - 23:00)
            // Bitiş saati de 23:00'ü geçmemeli
            var appointmentEndTime = appointment.AppointmentDate.AddMinutes(selectedService?.Duration ?? 0);
            
            if (appointment.AppointmentDate.Hour < 9 || appointmentEndTime.Hour >= 23 || (appointmentEndTime.Hour == 23 && appointmentEndTime.Minute > 0))
            {
                ModelState.AddModelError("", "Salonumuz 09:00 - 23:00 saatleri arasında hizmet vermektedir. Seçtiğiniz hizmetin süresi kapanış saatini aşıyor.");
            }

            // --- 3. ÇAKIŞMA KONTROLÜ (OVERLAP CHECK) ---
            if (selectedService != null && ModelState.IsValid)
            {
                // Seçilen hocanın, o günkü onaylanmış veya bekleyen randevularını getir
                // (Reddedilenler çakışma yaratmaz)
                var existingAppointments = await _context.Appointments
                    .Include(a => a.Service) // Süre bilgisini almak için Service dahil et
                    .Where(a => a.TrainerId == appointment.TrainerId 
                                && a.AppointmentDate.Date == appointment.AppointmentDate.Date
                                && a.Status != "Reddedildi")
                    .ToListAsync();

                foreach (var existing in existingAppointments)
                {
                    // Mevcut randevunun başlangıç ve bitişi
                    var existStart = existing.AppointmentDate;
                    var existEnd = existing.AppointmentDate.AddMinutes(existing.Service.Duration);

                    // Yeni randevunun başlangıç ve bitişi
                    var newStart = appointment.AppointmentDate;
                    var newEnd = newStart.AddMinutes(selectedService.Duration);

                    // ÇAKIŞMA FORMÜLÜ: (YeniBaşla < EskiBit) VE (YeniBit > EskiBaşla)
                    if (newStart < existEnd && newEnd > existStart)
                    {
                        ModelState.AddModelError("", $"Seçtiğiniz antrenörün {existStart:HH:mm} - {existEnd:HH:mm} saatleri arasında başka bir randevusu var. Lütfen başka bir saat seçiniz.");
                        break; // İlk çakışmada döngüden çık
                    }
                }
            }

            // --- 4. KAYIT İŞLEMİ ---
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                appointment.AppUserId = user.Id;
                appointment.Status = "Bekliyor"; // Onay mekanizması için başlangıç durumu

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Randevu talebiniz başarıyla alındı! Yönetici onayı bekleniyor.";
                return RedirectToAction("Index");
            }

            // Hata varsa listeleri tekrar doldur
            ViewBag.Trainers = _context.Trainers.ToList();
            ViewBag.Services = _context.Services.ToList();
            return View(appointment);
        }

        // Randevu İptal
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu iptal edildi.";
            }
            return RedirectToAction("Index");
        }
    }
}