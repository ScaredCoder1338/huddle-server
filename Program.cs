using HuddleServer.Hubs;
using HuddleServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=huddle.db"));

// Оптимизация SignalR для уменьшения задержки
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
});

builder.Services.AddControllers(); // Добавляем контроллеры
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Create database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Создаем базу данных если её нет
        db.Database.EnsureCreated();
        Console.WriteLine("✅ База данных готова");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Ошибка создания базы данных: {ex.Message}");
    }
}

app.UseCors("AllowAll");
app.UseStaticFiles(); // Для раздачи загруженных файлов
app.MapControllers(); // Добавляем маршруты контроллеров
app.MapHub<ChatHub>("/chathub");

// Получаем порт из переменной окружения или используем 5000 по умолчанию
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var url = $"http://0.0.0.0:{port}";

Console.WriteLine($"🚀 Huddle Server запущен на {url}");
Console.WriteLine($"📡 SignalR Hub: {url}/chathub");

app.Run(url);
