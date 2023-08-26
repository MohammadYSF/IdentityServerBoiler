using Domain.Entities;
using Domain.ViewModels;

namespace Application.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResponseDTO> SignIn(LoginRequestDTO loginRequestDTO, bool isPresistent);
        Task<ApplicationUser> Register(ApplicationUser user, string password);
        Task<ApplicationUser> GetByEmail(string email, CancellationToken cancellationToken);

    }
}
