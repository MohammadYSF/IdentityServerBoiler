namespace Domain.ViewModels
{
    public sealed record LoginResponseDTO(string FullName, string AccessToken , string RefreshToken);
}
