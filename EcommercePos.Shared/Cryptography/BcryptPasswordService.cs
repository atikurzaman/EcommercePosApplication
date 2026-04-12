using BC = BCrypt.Net.BCrypt;

namespace EcommercePos.Shared.Cryptography;

/// <summary>
/// Bcrypt-based password hashing service.
/// Uses work factor 12 (2^12 = 4096 iterations) - defends against GPU/ASIC attacks.
/// </summary>
public class BcryptPasswordService : IPasswordService
{
    /// <summary>
    /// Work factor for Bcrypt. Higher = slower (more resistant to brute force).
    /// 12 = ~0.3 seconds per hash on modern hardware (good balance).
    /// </summary>
    private const int WorkFactor = 12;

    /// <summary>
    /// Hash a plaintext password using Bcrypt.
    /// Each call generates a new salt automatically.
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BC.HashPassword(password, workFactor: WorkFactor);
    }

    /// <summary>
    /// Verify a plaintext password against a Bcrypt hash.
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BC.Verify(password, hash);
        }
        catch
        {
            // If hash is corrupted or invalid, return false
            return false;
        }
    }
}
