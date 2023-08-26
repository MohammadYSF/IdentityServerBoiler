using Domain.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserFeatures.Login
{
    public sealed class LoginValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().MaximumLength(50).EmailAddress();
            RuleFor(x => x.Password).NotNull().NotEmpty();
            RuleFor(x=> x.ClientId).NotNull().NotEmpty();
            RuleFor(x=> x.ClientSecret).NotNull().NotEmpty();


        }
    }
}
