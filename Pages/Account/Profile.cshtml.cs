using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Account;

public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public ProfileModel(ApplicationDbContext db) { _db = db; }
    public User? ProfileUser { get; set; }
    public List<Advertisement> Ads { get; set; } = new();

    public async Task OnGetAsync(int userId)
    {
        ProfileUser = await _db.Users.FindAsync(userId);
        if (ProfileUser != null)
            Ads = await _db.Advertisements.Include(a => a.Category).Include(a => a.Location)
                .Where(a => a.UserID == userId && a.Status == "Active")
                .OrderByDescending(a => a.PublishedDate).ToListAsync();
    }
}
