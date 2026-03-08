using Microsoft.EntityFrameworkCore;
using HuddleServer.Models;

namespace HuddleServer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SenderId).IsRequired();
            entity.Property(e => e.SenderUsername).IsRequired();
            entity.Property(e => e.ReceiverId).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();
        });
    }
}
