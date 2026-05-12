using Microsoft.AspNetCore.Mvc;

namespace WhatToCook.BLL.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");
            // Визначаємо шлях до wwwroot: якщо WebRootPath null, беремо ContentRootPath + "wwwroot"
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            // Тепер формуємо шлях до папки images
            var uploadsPath = Path.Combine(webRootPath, "images");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // Генеруємо унікальне ім'я для файлу, щоб уникнути конфліктів
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            // Зберігаємо файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Повертаємо URL до файлу (наприклад: https://твій-сервер/images/фото.jpg)
            var baseUrl = $"https://5q419q9m-7086.euw.devtunnels.ms/";
            return Ok(new { url = $"{baseUrl}/images/{fileName}" });
        }
    }
}