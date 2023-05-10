using FluentAssertions;
using WebApiVK.Authorization;
using WebApiVK.Interfaces;

namespace Tests.Unit;

public class EncryptorSha256Tests
{
    private readonly IEncryptor encryptor;

    public EncryptorSha256Tests()
    {
        encryptor = new EncryptorSha256();
    }

    [Fact]
    public void SamePassword_ShouldReturnSameHash()
    {
        var password = "TestPassword";

        var hash1 = encryptor.EncryptPassword(password);
        var hash2 = encryptor.EncryptPassword(password);

        hash1.Should().Be(hash2);
    }

    [Fact]
    public void DifferentPassword_ShouldReturnOtherHash()
    {
        var password1 = "TestPassword1";
        var password2 = "TestPassword2";

        var hash1 = encryptor.EncryptPassword(password1);
        var hash2 = encryptor.EncryptPassword(password2);

        hash1.Should().NotBe(hash2);
    }
}