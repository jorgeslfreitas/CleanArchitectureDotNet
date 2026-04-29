using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "Admin";
    public bool IsActive { get; private set; }

    public User() { }

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Validate(email, passwordHash);
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }

    public void UpdatePassword(string passwordHash)
    {
        Validate(Email, passwordHash);
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleException("Email is required");

        if (email.Length > 200)
            throw new BusinessRuleException("Email must be at most 200 characters");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new BusinessRuleException("Password hash is required");

        if (passwordHash.Length > 500)
            throw new BusinessRuleException("Password hash must be at most 500 characters");
    }
}