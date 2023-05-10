using FluentAssertions;
using WebApiVK.Authorization;

namespace Tests.Unit;

public class Base64CoderTests
{
    private Base64Coder coder = new();

    [Theory]
    [InlineData("admin", "admin", "YWRtaW46YWRtaW4=")]
    [InlineData("symbols_+-/", "--=Sym=--", "c3ltYm9sc18rLS86LS09U3ltPS0t")]
    public void EncodeCredentials_ShouldReturnCorrectString(string login, string password, string expected)
    {
        var encoded = coder.EncodeCredentials(login, password);

        encoded.Should().Be(expected);
    }

    [Theory]
    [InlineData("admin", "admin", "YWRtaW46YWRtaW4=")]
    [InlineData("symbols_+-/", "--=Sym=--", "c3ltYm9sc18rLS86LS09U3ltPS0t")]
    public void DecodeCredentials_ShouldReturnCorrectString(string login, string password, string encodedCredentials)
    {
        var encoded = coder.DecodeCredentials(encodedCredentials);

        encoded.Item1.Should().Be(login);
        encoded.Item2.Should().Be(password);
    }
}