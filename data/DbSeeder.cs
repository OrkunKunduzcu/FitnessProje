using FitnessProje.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace FitnessProje.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // Kullanıcı yönetimi ve Rol yönetimi servislerini çağırıyoruz
            var userManager = service.GetService<UserManager<AppUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. Roller veritabanında var mı? Yoksa oluştur.
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));

            // 2. Admin kullanıcısı 
            string adminEmail = "G231210051@sakarya.edu.tr"; 
            
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Orkun Kunduzcu", 
                    EmailConfirmed = true,
                    Gender = "Erkek"
                };

                // Şifre: sau 
                var result = await userManager.CreateAsync(newAdmin, "sau");
                
                if (result.Succeeded)
                {
                    // Kullanıcı başarıyla oluştuysa ona Admin rolünü ver
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
} 
