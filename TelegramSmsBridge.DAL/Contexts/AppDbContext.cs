using Microsoft.EntityFrameworkCore;
using TelegramSmsBridge.DAL.Entities;

namespace TelegramSmsBridge.DAL.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.TelegramUserName, "idx_telegram_user_name").IsUnique();

            entity.HasIndex(e => e.TelegramUserName, "users_telegram_user_name_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(32)
                .HasColumnName("refresh_token");
            entity.Property(e => e.RefreshTokenLastUpdated).HasColumnName("refresh_token_last_updated");
            entity.Property(e => e.TelegramUserName)
                .HasMaxLength(32)
                .HasColumnName("telegram_user_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
