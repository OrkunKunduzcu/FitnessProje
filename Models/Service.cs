using System.ComponentModel.DataAnnotations;

namespace FitnessProje.Web.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Display(Name = "Hizmet Adı")]
        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        public string Name { get; set; } // Örn: Yoga, Pilates, Fitness

        [Display(Name = "Süre (Dakika)")]
        [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
        public int Duration { get; set; } 

        [Display(Name = "Ücret (TL)")]
        [Required(ErrorMessage = "Ücret bilgisi zorunludur.")]
        public decimal Price { get; set; }
    }
}