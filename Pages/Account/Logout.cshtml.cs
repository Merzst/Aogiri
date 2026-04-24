using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aogiri.Pages.Account;

public class LogoutModel : PageModel
{
    public IActionResult OnPost()
    {
        HttpContext.Session.Clear();
        TempData["Success"] = "Вы вышли из системы.";
        return RedirectToPage("/Index");
    }
}
