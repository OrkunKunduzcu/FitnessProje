using FitnessProje.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessProje.Web.Controllers
{
    // Bu etiketi ekleyince sistem bunun bir API olduğunu anlar
    [Route("api/[controller]")]
    [ApiController]
    public class WebApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WebApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ANTRENÖRLERİ GETİREN API (JSON Döner)
        // Adres: /api/webapi/trainers
        [HttpGet("trainers")]
        public async Task<IActionResult> GetTrainers()
        {
            var trainers = await _context.Trainers
                .Select(t => new 
                {
                    t.Id,
                    t.FullName,
                    t.Expertise,
                    // Resim yolunu tam adres olarak verelim ki mobilde de açılsın
                    ImageUrl = "https://localhost:5292" + t.ImageUrl 
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // 2. TÜM HİZMETLERİ GETİREN API
        // Adres: /api/webapi/services
        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await _context.Services.ToListAsync();
            return Ok(services);
        }
    }
}
