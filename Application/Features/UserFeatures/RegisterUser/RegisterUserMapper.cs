using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserFeatures.RegisterUser
{
    public class LoginMapper : Profile
    {
        public LoginMapper()
        {
            CreateMap<RegisterUserRequestDTO, ApplicationUser>().ForMember(dest => dest.UserName , opt => opt.MapFrom(src => src.Email));
            CreateMap<ApplicationUser, RegisterUserResponseDTO>();
        }
    }
}
