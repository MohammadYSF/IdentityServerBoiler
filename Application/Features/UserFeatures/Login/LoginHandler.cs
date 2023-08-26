using AutoMapper;
using Application.Repositories;
using Domain.Entities;
using MediatR;
using Domain.ViewModels;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Application.Features.UserFeatures.Login
{
    public sealed class LoginHandler : IRequestHandler<LoginRequestDTO, LoginResponseDTO>
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginHandler(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<LoginResponseDTO> Handle(LoginRequestDTO request, CancellationToken cancellationToken)
        {
            var response = await _authRepository.SignIn(request, false);
            return response;

        }
    }
}
