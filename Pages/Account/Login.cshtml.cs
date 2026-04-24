using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Aogiri.Services;

namespace Aogiri.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IAuthService _auth;
    public LoginModel(IAuthService auth) { _auth = auth; }

    [BindProperty] public string Login { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        { ErrorMessage = "Заполните все поля"; return Page(); }

        var user = await _auth.LoginAsync(Login, Password);
        if (user == null) { ErrorMessage = "Неверный логин или пароль"; return Page(); }
        if (user.Status == "Blocked") { ErrorMessage = "Ваш аккаунт заблокирован"; return Page(); }

        HttpContext.Session.SetInt32("UserId", user.UserID);
        HttpContext.Session.SetString("UserName", user.Name);
        HttpContext.Session.SetString("UserRole", user.Role);
        await _auth.LogActivityAsync(user.UserID, "Вход в систему");
        TempData["Success"] = $"Добро пожаловать, {user.Name}!";
        return RedirectToPage("/Index");
    }
}
