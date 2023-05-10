using FakeItEasy;
using FluentAssertions;
using WebApiVK.Authorization;
using WebApiVK.Models;
using WebApiVK.Validators;

namespace Tests.Unit;

public class UserToCreateDtoValidatorTests
{
    private Base64Coder coder = new();

    [Theory]
    [InlineData("low", "normal", "Login", "Login must be between 4 and 30 characters.")]
    [InlineData("very_very_very_very_very_long_string", "normal", "Login", "Login must be between 4 and 30 characters.")]
    [InlineData("normal", "low", "Password", "Password must be between 4 and 30 characters.")]
    [InlineData("normal", "very_very_very_very_very_long_string", "Password", "Password must be between 4 and 30 characters.")]
    [InlineData("", "normal", "Login", "Login is required.")]
    [InlineData("normal", "", "Password", "Password is required.")]
    [InlineData("login with spaces", "normal", "Login", "Login must be without space characters.")]
    [InlineData("normal", "pass with spaces", "Password", "Password must be without space characters.")]
    public void WhenCredentialsNotValid_ShouldReturnErrorWithMessage(string login,
        string password,
        string propName,
        string errorMessage)
    {
        var validator = new UserToCreateDtoValidator(new Base64Coder());
        var model = new UserToCreateDto(coder.Encode(login), coder.Encode(password));
        var result = validator.Validate(model);

        result.Errors
            .Should()
            .Contain(r => r.PropertyName == propName &&
                          r.ErrorMessage == errorMessage);
    }

    [Theory]
    [InlineData("normal", "normal")]
    [InlineData("normal:login+-/", "normal-password+-*")]
    public void ValidInput_ShouldNotReturnErrors(string login, string password)
    {
        var validator = new UserToCreateDtoValidator(new Base64Coder());
        var model = new UserToCreateDto(coder.Encode(login), coder.Encode(password));
        var result = validator.Validate(model);

        result.Errors.Should().BeEmpty();
    }
}