using WebApiVK.Interfaces;

namespace WebApiVK.Domain;

public class LoginQueueManager : ILoginsManager
{
    private HashSet<string> queue = new HashSet<string>();
    private SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public bool TryAddLoginToQueue(string login)
    {
        semaphore.Wait();

        if (!queue.Contains(login))
        {
            queue.Add(login);
            semaphore.Release();
            return true;
        }

        semaphore.Release();
        return false;
    }

    public bool TryRemoveLogin(string login)
    {
        semaphore.Wait();

        if (queue.Contains(login))
        {
            semaphore.Release();
            queue.Remove(login);
            return true;
        }

        semaphore.Release();
        return false;
    }

    public bool IsLoginInQueue(string login)
    {
        semaphore.Wait();

        if (queue.Contains(login))
        {
            semaphore.Release();
            return true;
        }

        semaphore.Release();
        return false;
    }
}