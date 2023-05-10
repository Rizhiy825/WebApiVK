namespace WebApiVK.Interfaces;

public interface ILoginsManager
{
    public bool TryAddLoginToQueue(string login);
    public bool TryRemoveLogin(string login);
    public bool IsLoginInQueue(string login);
}