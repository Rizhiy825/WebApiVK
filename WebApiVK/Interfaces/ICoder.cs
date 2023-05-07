using WebApiVK.Models;

namespace WebApiVK.Interfaces;

public interface ICoder
{
    public string EncodeCredentials(string login, string password);
    public (string, string) DecodeCredentials(string base64AuthStr);
    public string Encode(string input);
    public string Decode(string input);
}