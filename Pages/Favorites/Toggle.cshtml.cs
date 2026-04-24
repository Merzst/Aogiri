using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Favorites;

public class ToggleModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public ToggleModel(ApplicationDbContext db) { _db = db; }

    public async Task<IActionResult> OnPostAsync(int adId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        var existing = await _db.Favorites.FirstOrDefaultAsync(f => f.UserID == uid && f.AdID == adId);
        if (existing != null) { _db.Favorites.Remove(existing); TempData["Success"] = "Удалено из избранного"; }
        else { _db.Favorites.Add(new Favorite { UserID = uid.Value, AdID = adId, AddTime = DateTime.UtcNow }); TempData["Success"] = "Добавлено в избранное"; }
        await _db.SaveChangesAsync();
        return RedirectToPage("/Ads/Detail", new { id = adId });
    }
}
