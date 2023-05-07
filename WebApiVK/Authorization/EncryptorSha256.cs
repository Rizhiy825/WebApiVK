using System.Security.Cryptography;
using System.Text;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Authorization;

public class EncryptorSha256 : IEncryptor
{
    private const string salt = "randomSalt";

    // Получаем хэш пароля, подмешивая соль.
    public string EncryptPassword(string password)
    {
        byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
        saltBytes.CopyTo(combinedBytes, 0);
        passwordBytes.CopyTo(combinedBytes, saltBytes.Length);

        var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(combinedBytes);

        return Convert.ToBase64String(hashBytes);
    }
}