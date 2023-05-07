using FluentValidation;
using WebApiVK.Interfaces;
using WebApiVK.Models;

namespace WebApiVK.Domain;

public class UserToCreateDtoValidator : AbstractValidator<UserToCreateDto>
{
    public UserToCreateDtoValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login is required.")
            .Length(4, 30).WithMessage("Login must be between 4 and 30 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(4, 30).WithMessage("Password must be between 4 and 30 characters.");
    }
}