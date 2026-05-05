using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Hubs;
using Aogiri.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHostedService<AdExpiryService>();

// ── SignalR ──────────────────────────────────────────────────
builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AogiriSession";
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

// ── Маршрут хаба ────────────────────────────────────────────
app.MapHub<ChatHub>("/chatHub");

// ── Миграции и папка uploads ─────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

    try { db.Database.Migrate(); } catch { /* игнорируем если уже применены */ }

    // Гарантируем существование папки для загрузок
    var uploadsPath = Path.Combine(env.WebRootPath, "uploads");
    if (!Directory.Exists(uploadsPath))
        Directory.CreateDirectory(uploadsPath);
}

app.Run();