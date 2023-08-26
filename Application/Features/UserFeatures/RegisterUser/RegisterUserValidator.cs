using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserFeatures.RegisterUser
{
    public sealed class LoginValidator : AbstractValidator<RegisterUserRequestDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().MaximumLength(50).EmailAddress();
            RuleFor(x => x.FirstName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches("[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
            .Matches("[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least 1 digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least 1 non-alphanumeric character");

        }
    }
}
