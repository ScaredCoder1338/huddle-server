namespace HuddleServer.Models;

public class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderUsername { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    
    // Тип сообщения: text, image, video
    public string MessageType { get; set; } = "text";
    
    // URL для медиа файлов
    public string? MediaUrl { get; set; }
}
