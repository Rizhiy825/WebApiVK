using FluentAssertions;
using WebApiVK.Domain;

namespace Tests;

public class LoginQueueManagerTests
{
    [Fact]
    public void TryAddLogin_SameUserFromDifferentFlows_ShouldAddOnlyOneLogin()
    {
        var manager = new LoginQueueManager();
        var login = "testLogin";

        var addedCounter = 0;

        var first = new Thread(() =>
        {
            var isAdded = manager.TryAddLoginToQueue(login);
            if (isAdded) addedCounter++;
        });

        var second = new Thread(() =>
        {
            var isAdded = manager.TryAddLoginToQueue(login);
            if (isAdded) addedCounter++;
        });

        first.Start();
        second.Start();
        
        first.Join();
        second.Join();

        addedCounter.Should().Be(1);
    }

    [Fact]
    public void IsLoginInQueue_LoginNotInQueue_ShouldReturnFalse()
    {
        var manager = new LoginQueueManager();
        var login = "testLogin";

        var isInQueue = manager.IsLoginInQueue(login);

        isInQueue.Should().BeFalse();
    }

    [Fact]
    public void IsLoginInQueue_LoginInQueue_ShouldReturnTrue()
    {
        var manager = new LoginQueueManager();
        var login = "testLogin";

        manager.TryAddLoginToQueue(login);
        var isInQueue = manager.IsLoginInQueue(login);

        isInQueue.Should().BeTrue();
    }

    [Fact]
    public void RemoveLogin_LoginNotInQueue_ShouldReturnFalse()
    {
        var manager = new LoginQueueManager();
        var login = "testLogin";
        
        var isRemoved = manager.TryRemoveLogin(login);

        isRemoved.Should().BeFalse();
    }

    [Fact]
    public void RemoveLogin_LoginInQueue_ShouldRemoveLogin()
    {
        var manager = new LoginQueueManager();
        var login = "testLogin";

        manager.TryAddLoginToQueue(login);
        var isRemoved = manager.TryRemoveLogin(login);

        isRemoved.Should().BeTrue();
    }
}