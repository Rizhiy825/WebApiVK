namespace WebApiVK.Authorization;

public interface IEncryptor
{
    public string EncryptPassword(string password);
}