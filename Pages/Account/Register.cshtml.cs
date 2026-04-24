using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Aogiri.Services;

namespace Aogiri.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IAuthService _auth;
    public RegisterModel(IAuthService auth) { _auth = auth; }

    [BindProperty] public string Login { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    [BindProperty] public string Name { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public string? Email { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone))
        { ErrorMessage = "Заполните все обязательные поля"; return Page(); }
        if (Password.Length < 4) { ErrorMessage = "Пароль должен содержать минимум 4 символа"; return Page(); }

        var (ok, err) = await _auth.RegisterAsync(Login, Password, Name, Phone, Email);
        if (!ok) { ErrorMessage = err; return Page(); }

        TempData["Success"] = "Регистрация успешна! Войдите в систему.";
        return RedirectToPage("/Account/Login");
    }
}
