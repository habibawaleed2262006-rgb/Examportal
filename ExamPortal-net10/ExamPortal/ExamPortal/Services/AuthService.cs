using ExamPortal.Models;

namespace ExamPortal.Services;

// Scoped: one instance per browser circuit (per logged-in tab).
public class AuthService
{
    private readonly List<AppUser> _users = new()
    {
        new AppUser { Username = "admin",   Password = "admin123",   Role = "Admin" },
        new AppUser { Username = "student", Password = "student123", Role = "Student" },
    };

    public AppUser? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    public bool IsStudent => CurrentUser?.Role == "Student";

    public event Action? OnChange;

    public bool Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username?.Trim(), StringComparison.OrdinalIgnoreCase) && u.Password == password);
        if (user == null) return false;
        CurrentUser = user;
        OnChange?.Invoke();
        return true;
    }

    public void Logout()
    {
        CurrentUser = null;
        OnChange?.Invoke();
    }
}
