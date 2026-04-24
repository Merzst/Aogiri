using Aogiri.Data;
using Aogiri.Models;
using Microsoft.EntityFrameworkCore;

namespace Aogiri.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string login, string password);
    Task<(bool success, string error)> RegisterAsync(string login, string password, string name, string phone, string? email);
    Task LogActivityAsync(int? userId, string action, string? details = null, string result = "Success");
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User?> LoginAsync(string login, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (user == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
        if (user.Status == "Blocked") return null;
        return user;
    }

    public async Task<(bool success, string error)> RegisterAsync(string login, string password, string name, string phone, string? email)
    {
        if (await _db.Users.AnyAsync(u => u.Login == login))
            return (false, "Пользователь с таким логином уже существует");

        var user = new User
        {
            Login = login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Name = name,
            Phone = phone,
            Email = email,
            Role = "User",
            Status = "Active",
            RegDate = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task LogActivityAsync(int? userId, string action, string? details = null, string result = "Success")
    {
        _db.ActivityLogs.Add(new ActivityLog
        {
            UserID = userId,
            Action = action,
            Details = details,
            Result = result,
            Timestamp = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}
