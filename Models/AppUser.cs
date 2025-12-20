using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessProje.Web.Models
{
    // IdentityUser sınıfından miras alıyoruz, böylece şifre, email vb. hazır geliyor.
    public class AppUser : IdentityUser
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        public string FullName { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        // Yapay zeka hesaplamaları için gerekli alanlar
        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; } // Erkek / Kadın

        [Display(Name = "Boy (cm)")]
        public int? Height { get; set; }

        [Display(Name = "Kilo (kg)")]
        public double? Weight { get; set; }
    }
}// Model güncellendi
 
