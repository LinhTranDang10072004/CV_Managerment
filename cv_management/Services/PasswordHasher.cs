using System.Security.Cryptography;
using System.Text;

namespace cv_management.Services;

public interface IPasswordHasher
{
    bool VerifyPassword(string plaintextPassword, string storedSha256Hex);
    string ComputeHash(string plaintextPassword);
}

public class Sha256PasswordHasher : IPasswordHasher
{
    public bool VerifyPassword(string plaintextPassword, string storedSha256Hex)
    {
        var computed = ComputeHash(plaintextPassword);
        return string.Equals(computed, storedSha256Hex, StringComparison.OrdinalIgnoreCase);
    }

    public string ComputeHash(string plaintextPassword)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(plaintextPassword);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}


