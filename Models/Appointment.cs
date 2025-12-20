using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessProje.Web.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // İlişkiler: Hangi Üye?
        public string AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUser? AppUser { get; set; }

        // İlişkiler: Hangi Antrenör?
        [Display(Name = "Antrenör Seçimi")]
        [Required(ErrorMessage = "Lütfen bir antrenör seçiniz.")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        // İlişkiler: Hangi Hizmet?
        [Display(Name = "Hizmet Türü")]
        [Required(ErrorMessage = "Lütfen bir hizmet seçiniz.")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        [Display(Name = "Randevu Tarihi")]
        [Required(ErrorMessage = "Tarih ve saat seçimi zorunludur.")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Durum")]
        public string Status { get; set; } = "Bekliyor"; // Bekliyor, Onaylandı, İptal

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
} 
