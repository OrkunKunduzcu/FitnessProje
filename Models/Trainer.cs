using System.ComponentModel.DataAnnotations;

namespace FitnessProje.Web.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Antrenör adı zorunludur.")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        [Required(ErrorMessage = "Uzmanlık alanı belirtilmelidir.")]
        public string Expertise { get; set; } 

        [Display(Name = "Fotoğraf Yolu")]
        public string? ImageUrl { get; set; }
    }
} 
