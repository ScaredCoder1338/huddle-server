using Microsoft.AspNetCore.Mvc;

namespace HuddleServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UpdateController : ControllerBase
{
    private const string CurrentVersion = "1.0.0";
    private const string DownloadUrl = "https://github.com/ScaredCoder1338/huddle-releases/releases/latest/download/Huddle.exe";

    [HttpGet("check")]
    public IActionResult CheckForUpdates()
    {
        // Здесь можно добавить логику проверки версии из базы данных
        // Пока возвращаем статичную информацию
        
        var updateInfo = new
        {
            version = CurrentVersion,
            downloadUrl = DownloadUrl,
            releaseNotes = @"Что нового в версии 1.0.0:
• Добавлены аватарки в сообщениях
• Просмотр профиля собеседника
• Галочки прочитано/не прочитано
• Серверное время для сообщений
• Исправлены ошибки",
            isRequired = false
        };

        return Ok(updateInfo);
    }

    [HttpGet("version")]
    public IActionResult GetCurrentVersion()
    {
        return Ok(new { version = CurrentVersion });
    }
}
