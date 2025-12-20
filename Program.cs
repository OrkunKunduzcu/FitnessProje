using FitnessProje.Web.Data;
using FitnessProje.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// PostgreSQL Tarih hatasını (DateTime) düzelten ayar:
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı (PostgreSQL)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Identity (Üyelik Sistemi) Ayarları
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3; 
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// MVC servisini ekle
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ============================================================
// EKSİK OLAN KISIM BURASIYDI (Admin Oluşturma Kodu)
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // DbSeeder dosyasını çağırıyoruz ki Admin hesabını oluştursun
    await DbSeeder.SeedRolesAndAdminAsync(services);
}
// ============================================================

// Hata yönetimi ve güvenlik
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();// App Entry Point Updated
 
 
