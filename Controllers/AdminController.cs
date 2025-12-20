using FitnessProje.Web.Data;
using FitnessProje.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProje.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        // 1. HİZMET (SERVICE) İŞLEMLERİ
        public IActionResult Services()
        {
            var services = _context.Services.ToList();
            return View(services);
        }

        public IActionResult CreateService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Add(service);
                _context.SaveChanges();
                TempData["Success"] = "Hizmet başarıyla eklendi!";
                return RedirectToAction("Services");
            }
            return View(service);
        }

        public IActionResult EditService(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return NotFound();
            return View(service);
        }

        [HttpPost]
        public IActionResult EditService(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Update(service);
                _context.SaveChanges();
                TempData["Success"] = "Hizmet başarıyla güncellendi!";
                return RedirectToAction("Services");
            }
            return View(service);
        }

        public IActionResult DeleteService(int id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
                TempData["Success"] = "Hizmet silindi.";
            }
            return RedirectToAction("Services");
        }

        // 2. ANTRENÖR (TRAINER) İŞLEMLERİ
        public IActionResult Trainers()
        {
            var trainers = _context.Trainers.ToList();
            return View(trainers);
        }

        public IActionResult CreateTrainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainer(Trainer trainer, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    trainer.ImageUrl = "/uploads/" + uniqueFileName;
                }
                else
                {
                    trainer.ImageUrl = "/img/default-user.png"; 
                }

                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Antrenör başarıyla eklendi!";
                return RedirectToAction("Trainers");
            }
            return View(trainer);
        }

        // --- ANTRENÖR DÜZENLEME (GET) ---
        public IActionResult EditTrainer(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer == null) return NotFound();
            return View(trainer);
        }

        // --- ANTRENÖR DÜZENLEME (POST) ---
        [HttpPost]
        public async Task<IActionResult> EditTrainer(Trainer trainer, IFormFile? imageFile)
        {
            
            if (ModelState.IsValid)
            {
                // Eğer yeni bir resim seçildiyse onu yükle ve güncelle
                if (imageFile != null)
                {
                    string extension = Path.GetExtension(imageFile.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + extension;
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    trainer.ImageUrl = "/uploads/" + uniqueFileName;
                }
                // Eğer resim seçilmediyse, hidden input'tan gelen eski ImageUrl korunur.

                _context.Trainers.Update(trainer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Antrenör bilgileri güncellendi!";
                return RedirectToAction("Trainers");
            }
            return View(trainer);
        }

        public IActionResult DeleteTrainer(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                _context.SaveChanges();
                TempData["Success"] = "Antrenör silindi.";
            }
            return RedirectToAction("Trainers");
        }

        // 3. RANDEVU YÖNETİMİ
        public IActionResult Appointments()
        {
            var appointments = _context.Appointments
                .Include(a => a.AppUser)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .OrderByDescending(a => a.Id) 
                .ToList();

            return View(appointments);
        }

        public IActionResult ApproveAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = "Onaylandı";
                _context.SaveChanges();
                TempData["Success"] = "Randevu onaylandı.";
            }
            return RedirectToAction("Appointments");
        }

        public IActionResult RejectAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = "Reddedildi";
                _context.SaveChanges();
                TempData["Success"] = "Randevu reddedildi.";
            }
            return RedirectToAction("Appointments");
        }

        // 4. KULLANICI (USER) İŞLEMLERİ
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult DeleteUser(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["Success"] = "Kullanıcı başarıyla silindi.";
            }
            return RedirectToAction("Users");
        }
    }
}