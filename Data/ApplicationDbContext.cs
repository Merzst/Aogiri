using Microsoft.EntityFrameworkCore;
using Aogiri.Models;

namespace Aogiri.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Advertisement> Advertisements { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<ModerationRule> ModerationRules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("User_1");
            e.HasKey(u => u.UserID);
            e.HasIndex(u => u.Login).IsUnique();
            e.Property(u => u.Status).HasDefaultValue("Active");
            e.Property(u => u.RegDate).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Category_2");
            e.HasKey(c => c.CategoryID);
        });

        modelBuilder.Entity<Location>(e =>
        {
            e.ToTable("Location_3");
            e.HasKey(l => l.LocationID);
        });

        modelBuilder.Entity<Advertisement>(e =>
        {
            e.ToTable("Advertisement_4");
            e.HasKey(a => a.AdID);
            e.Property(a => a.Price).HasColumnType("money");
            e.Property(a => a.Status).HasDefaultValue("Draft");
            e.Property(a => a.PublishedDate).HasDefaultValueSql("GETDATE()");
            e.HasOne(a => a.User).WithMany(u => u.Advertisements).HasForeignKey(a => a.UserID).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Location).WithMany(l => l.Advertisements).HasForeignKey(a => a.LocationID).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.Category).WithMany(c => c.Advertisements).HasForeignKey(a => a.CategoryID).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Message>(e =>
        {
            e.ToTable("Message_5");
            e.HasKey(m => m.MessageID);
            e.Property(m => m.SentDate).HasDefaultValueSql("GETDATE()");
            e.HasOne(m => m.Sender).WithMany(u => u.SentMessages).HasForeignKey(m => m.SenderID).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Receiver).WithMany(u => u.ReceivedMessages).HasForeignKey(m => m.ReceiverID).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Advertisement).WithMany(a => a.Messages).HasForeignKey(m => m.AdID).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Favorite>(e =>
        {
            e.ToTable("Favorite_6");
            e.HasKey(f => f.FavoriteID);
            e.Property(f => f.AddTime).HasDefaultValueSql("GETDATE()");
            e.HasOne(f => f.User).WithMany(u => u.Favorites).HasForeignKey(f => f.UserID).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(f => f.Advertisement).WithMany(a => a.Favorites).HasForeignKey(f => f.AdID).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivityLog>(e =>
        {
            e.ToTable("ActivityLog_7");
            e.HasKey(l => l.LogID);
            e.Property(l => l.Timestamp).HasDefaultValueSql("GETDATE()");
            e.HasOne(l => l.User).WithMany(u => u.ActivityLogs).HasForeignKey(l => l.UserID).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ModerationRule>(e =>
        {
            e.ToTable("ModerationRule_8");
            e.HasKey(r => r.RuleID);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>().HasData(
            new Location { LocationID = 1, City = "Полоцк", Region = "Витебская область", Country = "Беларусь" },
            new Location { LocationID = 2, City = "Минск", Region = "Минская область", Country = "Беларусь" },
            new Location { LocationID = 3, City = "Витебск", Region = "Витебская область", Country = "Беларусь" },
            new Location { LocationID = 4, City = "Гродно", Region = "Гродненская область", Country = "Беларусь" },
            new Location { LocationID = 5, City = "Брест", Region = "Брестская область", Country = "Беларусь" },
            new Location { LocationID = 6, City = "Гомель", Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID = 7, City = "Могилев", Region = "Могилевская область", Country = "Беларусь" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryID = 1, Name = "Транспорт", IconClass = "bi-car-front" },
            new Category { CategoryID = 2, Name = "Недвижимость", IconClass = "bi-house" },
            new Category { CategoryID = 3, Name = "Электроника", IconClass = "bi-laptop" },
            new Category { CategoryID = 4, Name = "Одежда и обувь", IconClass = "bi-bag" },
            new Category { CategoryID = 5, Name = "Работа", IconClass = "bi-briefcase" },
            new Category { CategoryID = 6, Name = "Услуги", IconClass = "bi-tools" },
            new Category { CategoryID = 7, Name = "Животные", IconClass = "bi-heart" },
            new Category { CategoryID = 8, Name = "Хобби и спорт", IconClass = "bi-bicycle" },
            new Category { CategoryID = 9, Name = "Мебель", IconClass = "bi-lamp" },
            new Category { CategoryID = 10, Name = "Другое", IconClass = "bi-three-dots" }
        );

        // Seed admin, moderator, and demo user
        // Хэши статические (pre-computed), $2a$ — совместимы с BCrypt.Net-Next всех версий.
        // admin / admin9999
        const string hashAdmin = "$2a$11$NKjkwTwLu6KLCU0On5/y4um4rbvXPAZ9EfYel3pz.3QCUK0z5BnA6";
        // moder / moder5678
        const string hashModer = "$2a$11$47bl0ES41utvX0yTFdDNEe3E4Qj2XGrx1uiVR9F1MYqzoTkTesgvi";
        // user1 / 1234
        const string hashUser1 = "$2a$11$NEWfnId0jt.xm1mfW8b8QewkDrwbufrntAaaPL0NVN.xhq99Z35vq";

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserID = 1,
                Login = "admin",
                Name = "Администратор",
                PasswordHash = hashAdmin,
                Phone = "+375291234567",
                Role = "Admin",
                Status = "Active",
                RegDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserID = 2,
                Login = "moder",
                Name = "Модератор",
                PasswordHash = hashModer,
                Phone = "+375297654321",
                Role = "Moderator",
                Status = "Active",
                RegDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                UserID = 3,
                Login = "user1",
                Name = "Иван Иванов",
                PasswordHash = hashUser1,
                Phone = "+375291111111",
                Role = "User",
                Status = "Active",
                RegDate = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<ModerationRule>().HasData(
            new ModerationRule { RuleID = 1, Phrase = "только пересылка", CreatedAt = new DateTime(2025, 1, 1) },
            new ModerationRule { RuleID = 2, Phrase = "бесплатно отдам", CreatedAt = new DateTime(2025, 1, 1) },
            new ModerationRule { RuleID = 3, Phrase = "казино", CreatedAt = new DateTime(2025, 1, 1) }
        );
    }
}