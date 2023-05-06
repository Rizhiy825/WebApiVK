using System.Security.Cryptography;
using System.Text;
using WebApiVK.Models;

namespace WebApiVK.Authorization;

public class UserService : IUserService
{
    private const string salt = "randomSalt";

    public Task<UserForAuthDTO> Authenticate(string username, string password)
    {
        if (username == "admin")
        {
            var passwordHash = GetPasswordHash(password);

            if (passwordHash == "WTgmRoHu2yk6gt1YN44y86vREhV6JLQTFkCZYoLpSgM=")
            {
                return Task<UserForAuthDTO>.Run(() => new UserForAuthDTO(){Id = new Guid(), Login = "admin"});
            }
        }

        return null;
    }

    // Получаем хэш пароля, подмешивая соль
    public string GetPasswordHash(string password)
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