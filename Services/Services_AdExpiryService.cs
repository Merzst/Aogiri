using Aogiri.Data;
using Microsoft.EntityFrameworkCore;

namespace Aogiri.Services;

/// <summary>
/// Фоновый сервис: каждый час деактивирует объявления,
/// у которых истёк срок публикации (ExpiryDate).
/// </summary>
public class AdExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AdExpiryService> _logger;

    public AdExpiryService(IServiceScopeFactory scopeFactory,
                            ILogger<AdExpiryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger      = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Первый запуск — немедленно после старта приложения
        await ExpireAdsAsync();

        // Затем каждый час
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ExpireAdsAsync();
        }
    }

    private async Task ExpireAdsAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expired = await db.Advertisements
                .Where(a => a.Status == "Active"
                         && a.ExpiryDate.HasValue
                         && a.ExpiryDate.Value < DateTime.UtcNow)
                .ToListAsync();

            if (expired.Count == 0) return;

            foreach (var ad in expired)
                ad.Status = "Inactive";

            await db.SaveChangesAsync();
            _logger.LogInformation(
                "AdExpiryService: деактивировано {Count} просроченных объявлений.", expired.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AdExpiryService: ошибка при деактивации объявлений.");
        }
    }
}
