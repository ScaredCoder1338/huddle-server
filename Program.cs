using HuddleServer.Hubs;
using HuddleServer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=huddle.db"));

builder.Services.AddSignalR();
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
    db.Database.Migrate(); // Используем миграции вместо EnsureCreated
}

app.UseCors("AllowAll");
app.MapHub<ChatHub>("/chathub");

// Получаем порт из переменной окружения или используем 5000 по умолчанию
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var url = $"http://0.0.0.0:{port}";

Console.WriteLine($"🚀 Huddle Server запущен на {url}");
Console.WriteLine($"📡 SignalR Hub: {url}/chathub");

app.Run(url);
