using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        // Отправить сообщение ТОЛЬКО получателю (не отправителю!)
        if (ConnectedUsers.TryGetValue(receiverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message.SenderUsername, message.Content, message.SenderId, message.Timestamp);
        }
        
        // НЕ отправляем обратно отправителю - он уже добавил сообщение локально
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

    public async Task<bool> RegisterNewUser(string username, string email, string passwordHash)
    {
        try
        {
            // Проверяем существует ли пользователь
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username || u.Email == email);

            if (existingUser != null)
            {
                return false; // Пользователь уже существует
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> LoginUser(string email, string passwordHash)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        return user;
    }

    public async Task<List<string>> SearchUsers(string searchQuery)
    {
        var users = await _context.Users
            .Where(u => u.Username.Contains(searchQuery))
            .Select(u => u.Username)
            .Take(10)
            .ToListAsync();
        return users;
    }

    public async Task<int> GetUserIdByUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user?.Id ?? 0;
    }

    public async Task MarkMessageAsRead(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            await _context.SaveChangesAsync();
            
            // Уведомляем отправителя что сообщение прочитано
            if (ConnectedUsers.TryGetValue(message.SenderId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("MessageRead", messageId);
            }
        }
    }

    public async Task<User?> GetUserProfile(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        return user;
    }

    public async Task NotifyProfileUpdate(string username)
    {
        // Уведомляем всех подключенных пользователей об обновлении профиля
        await Clients.All.SendAsync("ProfileUpdated", username);
    }
}
