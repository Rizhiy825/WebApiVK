using FluentValidation;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Domain;

public class UserToCreateDtoValidator : AbstractValidator<UserToCreateDto>
{
    private ICoder coder;

    public UserToCreateDtoValidator(ICoder coder)
    {
        this.coder = coder; 

        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .Must(ValidCredentialsLenght).WithMessage("Login must be between 4 and 30 characters.")
            .Must(ValidSymbolsInCredentials).WithMessage("Login must be without space characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Must(ValidCredentialsLenght).WithMessage("Password must be between 4 and 30 characters.")
            .Must(ValidSymbolsInCredentials).WithMessage("Password must be without space characters.");
    }

    private bool ValidCredentialsLenght(string credential)
    {
        var decoded = coder.Decode(credential);

        if (decoded.Length >= 4 && decoded.Length <= 30)
        {
            return true;
        }

        return false;
    }

    // Будем считать, что в логине и пароле не должно быть символов пробела 
    private bool ValidSymbolsInCredentials(string credential)
    {
        if (credential.Contains(' '))
        {
            return false;
        }

        return true;
    }
}