using FitnessProje.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FitnessProje.Web.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(int age, int weight, int height, string goal, string gender)
        {
            string apiKey = "api gelecek"; 

            string prompt = $"Sen profesyonel bir fitness antrenörüsün. " +
                            $"Danışan bilgileri: Cinsiyet: {gender}, Yaş: {age}, Kilo: {weight}kg, Boy: {height}cm. " +
                            $"Hedef: {goal}. " +
                            $"Bu kişiye özel, maddeler halinde detaylı bir 'Antrenman Programı' ve 'Beslenme Tavsiyeleri' hazırla. " +
                            $"Cevabı HTML formatında (<b>, <ul>, <li>, <br> etiketlerini kullanarak) ver.";

            // 'gemini-1.5-flash' yerine en yeni 'gemini-2.5-flash' modelini kullanıyoruz.
            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            using (var client = new HttpClient())
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseString);
                        
                        try 
                        {
                            // Gemini cevabı buradan dönüyor
                            string aiResponse = result.candidates[0].content.parts[0].text;
                            
                            double heightInMeters = height / 100.0;
                            double bmi = weight / (heightInMeters * heightInMeters);

                            ViewBag.Result = aiResponse;
                            ViewBag.Bmi = Math.Round(bmi, 1);
                            ViewBag.IsResult = true;
                        }
                        catch
                        {
                            ViewBag.Error = "Cevap geldi ama formatı okunamadı.";
                        }
                    }
                    else
                    {
                        // Hata olursa Google'ın mesajını göster
                        var errorDetails = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = $"Google Hata Döndü: {errorDetails}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Sistem Hatası: " + ex.Message;
                }
            }

            return View("Index");
        }
    }
}