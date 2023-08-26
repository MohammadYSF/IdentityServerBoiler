using MediatR;

namespace Domain.ViewModels
{
    public sealed record LoginRequestDTO : IRequest<LoginResponseDTO>
    {

        public string Email { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
