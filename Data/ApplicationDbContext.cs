using Microsoft.EntityFrameworkCore;
using Aogiri.Models;

namespace Aogiri.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User>           Users           { get; set; }
    public DbSet<Advertisement>  Advertisements  { get; set; }
    public DbSet<Category>       Categories      { get; set; }
    public DbSet<Subcategory>    Subcategories   { get; set; }
    public DbSet<Location>       Locations       { get; set; }
    public DbSet<Message>        Messages        { get; set; }
    public DbSet<Favorite>       Favorites       { get; set; }
    public DbSet<ActivityLog>    ActivityLogs    { get; set; }
    public DbSet<ModerationRule> ModerationRules { get; set; }
    public DbSet<Report>         Reports         { get; set; }
    public DbSet<AdImage>        AdImages        { get; set; }
    public DbSet<AdAttribute>    AdAttributes    { get; set; }

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

        modelBuilder.Entity<Subcategory>(e =>
        {
            e.ToTable("Subcategory_11");
            e.HasKey(s => s.SubcategoryID);
            e.HasOne(s => s.Category)
                .WithMany(c => c.Subcategories)
                .HasForeignKey(s => s.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Location>(e => { e.ToTable("Location_3"); e.HasKey(l => l.LocationID); });

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
            e.HasOne(a => a.Subcategory).WithMany(s => s.Advertisements).HasForeignKey(a => a.SubcategoryID).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<AdAttribute>(e =>
        {
            e.ToTable("AdAttribute_12");
            e.HasKey(a => a.AdAttributeID);
            e.HasOne(a => a.Advertisement)
                .WithMany(ad => ad.Attributes)
                .HasForeignKey(a => a.AdID)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<ModerationRule>(e => { e.ToTable("ModerationRule_8"); e.HasKey(r => r.RuleID); });

        modelBuilder.Entity<Report>(e =>
        {
            e.ToTable("Report_9");
            e.HasKey(r => r.ReportID);
            e.Property(r => r.CreatedAt).HasDefaultValueSql("GETDATE()");
            e.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserID).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.Advertisement).WithMany().HasForeignKey(r => r.AdID).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AdImage>(e =>
        {
            e.ToTable("AdImage_10");
            e.HasKey(i => i.AdImageID);
            e.HasOne(i => i.Advertisement)
                .WithMany(a => a.Images)
                .HasForeignKey(i => i.AdID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // ── Города Беларуси (расширенный список) ──────────────────────────
        modelBuilder.Entity<Location>().HasData(
            new Location { LocationID =  1, City = "Полоцк",        Region = "Витебская область",  Country = "Беларусь" },
            new Location { LocationID =  2, City = "Минск",         Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID =  3, City = "Витебск",       Region = "Витебская область",  Country = "Беларусь" },
            new Location { LocationID =  4, City = "Гродно",        Region = "Гродненская область",Country = "Беларусь" },
            new Location { LocationID =  5, City = "Брест",         Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID =  6, City = "Гомель",        Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID =  7, City = "Могилев",       Region = "Могилевская область",Country = "Беларусь" },
            new Location { LocationID =  8, City = "Новополоцк",    Region = "Витебская область",  Country = "Беларусь" },
            new Location { LocationID =  9, City = "Барановичи",    Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID = 10, City = "Бобруйск",      Region = "Могилевская область",Country = "Беларусь" },
            new Location { LocationID = 11, City = "Борисов",       Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 12, City = "Пинск",         Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID = 13, City = "Орша",          Region = "Витебская область",  Country = "Беларусь" },
            new Location { LocationID = 14, City = "Солигорск",     Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 15, City = "Мозырь",        Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID = 16, City = "Лида",          Region = "Гродненская область",Country = "Беларусь" },
            new Location { LocationID = 17, City = "Молодечно",     Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 18, City = "Жодино",        Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 19, City = "Слуцк",         Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 20, City = "Несвиж",        Region = "Минская область",    Country = "Беларусь" },
            new Location { LocationID = 21, City = "Жлобин",        Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID = 22, City = "Светлогорск",   Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID = 23, City = "Речица",        Region = "Гомельская область", Country = "Беларусь" },
            new Location { LocationID = 24, City = "Ошмяны",        Region = "Гродненская область",Country = "Беларусь" },
            new Location { LocationID = 25, City = "Волковыск",     Region = "Гродненская область",Country = "Беларусь" },
            new Location { LocationID = 26, City = "Слоним",        Region = "Гродненская область",Country = "Беларусь" },
            new Location { LocationID = 27, City = "Кобрин",        Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID = 28, City = "Пружаны",       Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID = 29, City = "Берёза",        Region = "Брестская область",  Country = "Беларусь" },
            new Location { LocationID = 30, City = "Дзержинск",     Region = "Минская область",    Country = "Беларусь" }
        );

        // ── Категории ──────────────────────────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryID =  1, Name = "Транспорт",       IconClass = "bi-car-front"   },
            new Category { CategoryID =  2, Name = "Недвижимость",    IconClass = "bi-house"       },
            new Category { CategoryID =  3, Name = "Электроника",     IconClass = "bi-laptop"      },
            new Category { CategoryID =  4, Name = "Одежда и обувь",  IconClass = "bi-bag"         },
            new Category { CategoryID =  5, Name = "Работа",          IconClass = "bi-briefcase"   },
            new Category { CategoryID =  6, Name = "Услуги",          IconClass = "bi-tools"       },
            new Category { CategoryID =  7, Name = "Животные",        IconClass = "bi-heart"       },
            new Category { CategoryID =  8, Name = "Хобби и спорт",   IconClass = "bi-bicycle"     },
            new Category { CategoryID =  9, Name = "Мебель",          IconClass = "bi-lamp"        },
            new Category { CategoryID = 10, Name = "Другое",          IconClass = "bi-three-dots"  }
        );

        // ── Подкатегории ───────────────────────────────────────────────────
        modelBuilder.Entity<Subcategory>().HasData(
            // Транспорт
            new Subcategory { SubcategoryID =  1, CategoryID = 1, Name = "Легковые автомобили",  IconClass = "bi-car-front"    },
            new Subcategory { SubcategoryID =  2, CategoryID = 1, Name = "Мотоциклы",            IconClass = "bi-bicycle"      },
            new Subcategory { SubcategoryID =  3, CategoryID = 1, Name = "Грузовики",            IconClass = "bi-truck"        },
            new Subcategory { SubcategoryID =  4, CategoryID = 1, Name = "Запчасти",             IconClass = "bi-gear"         },
            new Subcategory { SubcategoryID =  5, CategoryID = 1, Name = "Велосипеды",           IconClass = "bi-bicycle"      },

            // Недвижимость
            new Subcategory { SubcategoryID =  6, CategoryID = 2, Name = "Квартиры",             IconClass = "bi-building"     },
            new Subcategory { SubcategoryID =  7, CategoryID = 2, Name = "Дома и дачи",          IconClass = "bi-house"        },
            new Subcategory { SubcategoryID =  8, CategoryID = 2, Name = "Комнаты",              IconClass = "bi-door-open"    },
            new Subcategory { SubcategoryID =  9, CategoryID = 2, Name = "Земельные участки",    IconClass = "bi-geo"          },
            new Subcategory { SubcategoryID = 10, CategoryID = 2, Name = "Коммерческая",         IconClass = "bi-shop"         },

            // Электроника
            new Subcategory { SubcategoryID = 11, CategoryID = 3, Name = "Телефоны",             IconClass = "bi-phone"        },
            new Subcategory { SubcategoryID = 12, CategoryID = 3, Name = "Ноутбуки и ПК",        IconClass = "bi-laptop"       },
            new Subcategory { SubcategoryID = 13, CategoryID = 3, Name = "Планшеты",             IconClass = "bi-tablet"       },
            new Subcategory { SubcategoryID = 14, CategoryID = 3, Name = "Телевизоры",           IconClass = "bi-tv"           },
            new Subcategory { SubcategoryID = 15, CategoryID = 3, Name = "Аудио и видео",        IconClass = "bi-music-note"   },
            new Subcategory { SubcategoryID = 16, CategoryID = 3, Name = "Фото и видео",         IconClass = "bi-camera"       },

            // Одежда и обувь
            new Subcategory { SubcategoryID = 17, CategoryID = 4, Name = "Мужская одежда",       IconClass = "bi-person"       },
            new Subcategory { SubcategoryID = 18, CategoryID = 4, Name = "Женская одежда",       IconClass = "bi-person"       },
            new Subcategory { SubcategoryID = 19, CategoryID = 4, Name = "Детская одежда",       IconClass = "bi-star"         },
            new Subcategory { SubcategoryID = 20, CategoryID = 4, Name = "Обувь",                IconClass = "bi-bag"          },
            new Subcategory { SubcategoryID = 21, CategoryID = 4, Name = "Аксессуары",           IconClass = "bi-watch"        },

            // Работа
            new Subcategory { SubcategoryID = 22, CategoryID = 5, Name = "IT и интернет",        IconClass = "bi-code-slash"   },
            new Subcategory { SubcategoryID = 23, CategoryID = 5, Name = "Строительство",        IconClass = "bi-hammer"       },
            new Subcategory { SubcategoryID = 24, CategoryID = 5, Name = "Торговля",             IconClass = "bi-cart"         },
            new Subcategory { SubcategoryID = 25, CategoryID = 5, Name = "Медицина",             IconClass = "bi-heart-pulse"  },
            new Subcategory { SubcategoryID = 26, CategoryID = 5, Name = "Образование",          IconClass = "bi-book"         },

            // Услуги
            new Subcategory { SubcategoryID = 27, CategoryID = 6, Name = "Ремонт и строительство",IconClass = "bi-tools"       },
            new Subcategory { SubcategoryID = 28, CategoryID = 6, Name = "Красота и здоровье",   IconClass = "bi-scissors"     },
            new Subcategory { SubcategoryID = 29, CategoryID = 6, Name = "Репетиторство",        IconClass = "bi-book"         },
            new Subcategory { SubcategoryID = 30, CategoryID = 6, Name = "Перевозки",            IconClass = "bi-truck"        },

            // Животные
            new Subcategory { SubcategoryID = 31, CategoryID = 7, Name = "Собаки",               IconClass = "bi-heart"        },
            new Subcategory { SubcategoryID = 32, CategoryID = 7, Name = "Кошки",                IconClass = "bi-heart"        },
            new Subcategory { SubcategoryID = 33, CategoryID = 7, Name = "Птицы",                IconClass = "bi-feather"      },
            new Subcategory { SubcategoryID = 34, CategoryID = 7, Name = "Рыбки и аквариумы",   IconClass = "bi-droplet"      },

            // Хобби и спорт
            new Subcategory { SubcategoryID = 35, CategoryID = 8, Name = "Спортивный инвентарь", IconClass = "bi-trophy"       },
            new Subcategory { SubcategoryID = 36, CategoryID = 8, Name = "Туризм и отдых",       IconClass = "bi-backpack"     },
            new Subcategory { SubcategoryID = 37, CategoryID = 8, Name = "Книги и журналы",      IconClass = "bi-book"         },
            new Subcategory { SubcategoryID = 38, CategoryID = 8, Name = "Музыкальные инструменты",IconClass = "bi-music-note-beamed"},

            // Мебель
            new Subcategory { SubcategoryID = 39, CategoryID = 9, Name = "Спальня",              IconClass = "bi-lamp"         },
            new Subcategory { SubcategoryID = 40, CategoryID = 9, Name = "Гостиная",             IconClass = "bi-tv"           },
            new Subcategory { SubcategoryID = 41, CategoryID = 9, Name = "Кухня",                IconClass = "bi-cup-hot"      },
            new Subcategory { SubcategoryID = 42, CategoryID = 9, Name = "Офисная мебель",       IconClass = "bi-briefcase"    }
        );

        // ── Пользователи ───────────────────────────────────────────────────
        const string hashAdmin = "$2a$11$NKjkwTwLu6KLCU0On5/y4um4rbvXPAZ9EfYel3pz.3QCUK0z5BnA6";
        const string hashModer = "$2a$11$47bl0ES41utvX0yTFdDNEe3E4Qj2XGrx1uiVR9F1MYqzoTkTesgvi";
        const string hashUser1 = "$2a$11$NEWfnId0jt.xm1mfW8b8QewkDrwbufrntAaaPL0NVN.xhq99Z35vq";

        modelBuilder.Entity<User>().HasData(
            new User { UserID = 1, Login = "admin", Name = "Администратор", PasswordHash = hashAdmin, Phone = "+375291234567", Role = "Admin",     Status = "Active", RegDate = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc) },
            new User { UserID = 2, Login = "moder", Name = "Модератор",     PasswordHash = hashModer, Phone = "+375297654321", Role = "Moderator", Status = "Active", RegDate = new DateTime(2025,1,1,0,0,0,DateTimeKind.Utc) },
            new User { UserID = 3, Login = "user1", Name = "Иван Иванов",   PasswordHash = hashUser1, Phone = "+375291111111", Role = "User",      Status = "Active", RegDate = new DateTime(2025,1,2,0,0,0,DateTimeKind.Utc) }
        );

        modelBuilder.Entity<ModerationRule>().HasData(
            new ModerationRule { RuleID = 1, Phrase = "только пересылка", CreatedAt = new DateTime(2025,1,1) },
            new ModerationRule { RuleID = 2, Phrase = "бесплатно отдам",  CreatedAt = new DateTime(2025,1,1) },
            new ModerationRule { RuleID = 3, Phrase = "казино",           CreatedAt = new DateTime(2025,1,1) }
        );
    }
}
