using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace HuddleServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UpdateController : ControllerBase
{
    private const string CurrentVersion = "1.0.4";
    private const string DownloadUrl = "https://huddle-server-production-ea35.up.railway.app/api/update/download";
    private readonly IWebHostEnvironment _env;

    public UpdateController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("check")]
    public IActionResult CheckForUpdates()
    {
        // Проверяем наличие файла обновления
        var updatePath = Path.Combine(_env.ContentRootPath, "updates", "Huddle.exe");
        long fileSize = 0;
        
        if (System.IO.File.Exists(updatePath))
        {
            fileSize = new FileInfo(updatePath).Length;
        }
        
        var updateInfo = new
        {
            version = CurrentVersion,
            downloadUrl = DownloadUrl,
            releaseNotes = @"Что нового в версии 1.0.4:
✅ Убрана вся локальная логика - теперь все работает через сервер
✅ Сообщения сохраняются только на сервере
✅ Регистрация и вход только через сервер
✅ Улучшена стабильность работы
✅ Более понятные сообщения об ошибках
✅ Оптимизирована работа с сетью",
            isRequired = false,
            fileSize = fileSize
        };

        return Ok(updateInfo);
    }

    [HttpGet("version")]
    public IActionResult GetCurrentVersion()
    {
        return Ok(new { version = CurrentVersion });
    }

    [HttpGet("download")]
    public IActionResult DownloadUpdate()
    {
        try
        {
            var updatePath = Path.Combine(_env.ContentRootPath, "updates", "Huddle.exe");
            
            if (!System.IO.File.Exists(updatePath))
            {
                return NotFound(new { message = "Файл обновления не найден" });
            }

            var fileBytes = System.IO.File.ReadAllBytes(updatePath);
            return File(fileBytes, "application/octet-stream", "Huddle.exe");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Ошибка загрузки: {ex.Message}" });
        }
    }
}
