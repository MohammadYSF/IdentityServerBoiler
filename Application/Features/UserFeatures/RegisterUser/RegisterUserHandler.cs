using AutoMapper;
using Application.Repositories;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Repositories;

namespace Application.Features.UserFeatures.RegisterUser
{
    public sealed class LoginHandler : IRequestHandler<RegisterUserRequestDTO, RegisterUserResponseDTO>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;

        public LoginHandler(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _mapper = mapper;
        }

        public async Task<RegisterUserResponseDTO> Handle(RegisterUserRequestDTO request, CancellationToken cancellationToken)
        {
            var applicationUser = await _authRepository.GetByEmail(request.Email, cancellationToken);
            var registerUserResponseDTO = new RegisterUserResponseDTO();
            if (applicationUser is not null)
            {
                registerUserResponseDTO.Success = false;
                registerUserResponseDTO.ErrorMessages.Add("email", "duplicate email address");
                return registerUserResponseDTO;
            }

            var newApplicationUser = _mapper.Map<ApplicationUser>(request);
            newApplicationUser.DateCreated = DateTime.UtcNow;                        
            var result = await _authRepository.Register(newApplicationUser, request.Password);
            registerUserResponseDTO = _mapper.Map<RegisterUserResponseDTO>(result);
            registerUserResponseDTO.Success = true;
            return registerUserResponseDTO;
        }
    }
}
