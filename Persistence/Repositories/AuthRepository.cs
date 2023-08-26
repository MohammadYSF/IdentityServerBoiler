
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Domain.ViewModels;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static IdentityModel.OidcConstants;

namespace Persistence.Repositories
{

    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ApplicationUser> GetByEmail(string email, CancellationToken cancellationToken)
        {
            ApplicationUser applicatonUser = await _userManager.FindByEmailAsync(email);
            return applicatonUser;
        }

        public async Task<ApplicationUser> Register(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded is false)
            {
                var errorMessage = result.Errors.FirstOrDefault()?.Description;
                throw new Exception($"User registration failed: {errorMessage}");
            }
            await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role.User));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.UserName));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, Enum.GetName(typeof(Role), Role.User)));

            return user;

        }
        public async Task<LoginResponseDTO> SignIn(LoginRequestDTO lognRequestDTO, bool isPresistent)
        {

            string email = lognRequestDTO.Email;
            string password = lognRequestDTO.Password;
            string clientId = lognRequestDTO.ClientId;
            string clientSecret = lognRequestDTO.ClientSecret;
            string tokenEndpoint = _configuration["IdentityServer:Authority"] + "/connect/token";
            ApplicationUser user = await _userManager.FindByEmailAsync(email) ?? throw new Exception("Wrong User Credentials");
            SignInResult isPasswordCorrect = await _signInManager.PasswordSignInAsync(user.UserName, password, isPresistent, false);
            if (!isPasswordCorrect.Succeeded)
                throw new Exception("Wrong User Credentials");
            if (!user.EmailConfirmed)
                throw new Exception("Email isn`t confirmed");

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var prevClaims = await _userManager.GetClaimsAsync(user);
                var sayHiClaim = new Claim("permission", "say_hi");
                var claimsToAdd = new List<Claim>();
                if (prevClaims.Where(a => a.Type == sayHiClaim.Type && a.Value == sayHiClaim.Value).Any() is false)
                {
                    claimsToAdd.Add(sayHiClaim);
                }
                await _userManager.AddClaimsAsync(user, claimsToAdd);
            }
            await _signInManager.SignInAsync(user, isPersistent: isPresistent);
            var httpClient = _httpClientFactory.CreateClient();

            var formData = new Dictionary<string, string>
        {
            { "grant_type", "password" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "username", user.UserName },
            { "password",password },
            { "scope","openid profile API.read API.write offline_access"},
            // Add more key-value pairs as needed
        };

            var data = new FormUrlEncodedContent(formData);
            var tokenResponse = await httpClient.PostAsync(tokenEndpoint, data);
            if (tokenResponse.IsSuccessStatusCode)
            {
                // Handle success
                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var accessToken = (string)JObject.Parse(tokenContent)["access_token"];
                var refreshToken = (string)JObject.Parse(tokenContent)["refresh_token"];
                return new LoginResponseDTO(user.UserName, accessToken, refreshToken);

            }
            else
            {
                throw new HttpRequestException("Can not get the access token");
            }


        }
    }
}
