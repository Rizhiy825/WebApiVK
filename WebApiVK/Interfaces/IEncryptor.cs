namespace WebApiVK.Interfaces;

public interface IEncryptor
{
    public string EncryptPassword(string password);
}