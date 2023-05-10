using System.Text;
using WebApiVK.Interfaces;

namespace WebApiVK.Authorization;

public class Base64Coder : ICoder
{
    public string EncodeCredentials(string login, string password)
    {
        var authLine = $"{login}:{password}";
        var base64Line = Encode(authLine);

        return base64Line;
    }

    public (string, string) DecodeCredentials(string base64AuthStr)
    {
        var decodedCredentials = Decode(base64AuthStr)
            .Split(':');

        var login = decodedCredentials[0];
        var password = decodedCredentials[1];

        return (login, password);
    }

    public string Encode(string input)
    {
        var base64Line = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(input));

        return base64Line;
    }

    public string Decode(string input)
    {
        var decodedBytes = Convert.FromBase64String(input);
        var decoded = Encoding.UTF8
            .GetString(decodedBytes);
        
        return decoded;
    }
}