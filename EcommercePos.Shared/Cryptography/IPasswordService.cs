namespace EcommercePos.Shared.Cryptography;

/// <summary>
/// Service for secure password hashing and verification using Bcrypt.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hash a plaintext password using Bcrypt with work factor 12.
    /// </summary>
    /// <param name="password">Plaintext password to hash</param>
    /// <returns>Bcrypt hash (includes salt)</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify a plaintext password against a Bcrypt hash.
    /// </summary>
    /// <param name="password">Plaintext password to verify</param>
    /// <param name="hash">Bcrypt hash from database</param>
    /// <returns>True if password matches, false otherwise</returns>
    bool VerifyPassword(string password, string hash);
}
