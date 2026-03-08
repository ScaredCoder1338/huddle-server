using Microsoft.AspNetCore.SignalR;
using HuddleServer.Models;
using HuddleServer.Data;

namespace HuddleServer.Hubs;

public class ChatHub : Hub
{
    private static readonly Dictionary<string, string> ConnectedUsers = new();
    private readonly AppDbContext _context;

    public ChatHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (userId != null)
        {
            ConnectedUsers.Remove(userId);
            await Clients.All.SendAsync("UserDisconnected", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterUser(string userId, string username)
    {
        ConnectedUsers[userId] = Context.ConnectionId;
        await Clients.All.SendAsync("UserConnected", userId, username);
    }

    public async Task SendMessage(string senderId, string senderUsername, string receiverId, string content)
    {
        var message = new Message
        {
            SenderId = senderId,
            SenderUsername = senderUsername,
            ReceiverId = receiverId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        // Сохранить в базу данных
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Отправить сообщение получателю
        if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }

        // Отправить подтверждение отправителю
        await Clients.Caller.SendAsync("MessageSent", message);
    }

    public async Task<List<Message>> GetMessageHistory(string userId1, string userId2)
    {
        var messages = _context.Messages
            .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                       (m.SenderId == userId2 && m.ReceiverId == userId1))
            .OrderBy(m => m.Timestamp)
            .ToList();

        return messages;
    }
}
