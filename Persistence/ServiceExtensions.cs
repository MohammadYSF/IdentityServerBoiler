using Application.Repositories;
using Persistence.Context;
using Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.Storage;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Extensions;
using System.Linq.Expressions;
using IdentityModel;
using System.Security.Claims;

namespace Persistence;

public static class ServiceExtensions
{
    public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("IdentityServerBoilerDatabase");
        const string assembly = "Persistence";
        services.AddDbContext<ApplicationContext>(opt =>
        opt.UseNpgsql(connectionString,
        b => b.MigrationsAssembly(assembly)));
        services.AddOperationalDbContext(options =>
        {
            options.ConfigureDbContext = db => db.UseNpgsql(connectionString, b => b.MigrationsAssembly(assembly));
        });

        #region Identityserver4 and AspNetIdentity configs
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(config =>
        {
            config.Authority = configuration["IdentityServer:Authority"];
            config.Audience = "API";
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters()
            {
                //IssuerSigningKey = signingKey,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false, //TODO : signing signature should be validated
                ClockSkew = TimeSpan.Zero
            };
        });
        services.AddIdentityServer()
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryClients(new List<Client>
            {
                     new Client
                {
                AccessTokenLifetime = 600, //access token expiration : 5 minutes
                ClientId = "reactclient",
                ClientName = "react client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime =86400, //refresh token expiration : 24 hours
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RedirectUris = { configuration["IdentityServer:Authority"]+"/signin-oidc" },
                FrontChannelLogoutUri = configuration["IdentityServer:Authority"] + "/signout-oidc",
                PostLogoutRedirectUris = { configuration["IdentityServer:Authority"]+"/signout-callback-oidc" },
                ClientSecrets = { new Secret("reactclient".Sha256()) },
                AllowedScopes = { "openid","profile","API.read", "API.write","offline_access"},
                }
            }).AddInMemoryIdentityResources(new List<IdentityResource>
            {
                   new IdentityResources.OpenId(),
                new IdentityResources.Profile{
                        UserClaims = { JwtClaimTypes.Role }
                },
                new IdentityResource
                {
                    Name = "email",
                    UserClaims = new List<string>{"email"}
                },
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string>{"role"}
                },
                new IdentityResource
                {
                    Name = "permission",
                    UserClaims = new List<string>{ "permission" }
                }

            }).AddInMemoryApiScopes(new List<ApiScope>
            {
                        new ApiScope("API.read"),
                        new ApiScope("API.write")
            })
            .AddInMemoryApiResources(new List<ApiResource>
            {
                        new ApiResource("API")
                        {
                            Scopes = new List<string>{"API.read","API.write"},
                            ApiSecrets = new List<Secret>{new Secret("APIsecret".Sha256())},
                            UserClaims = new List<string>{"email","role","permission"}
                        }
            })

            .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b =>
        b.UseNpgsql(connectionString, opt => opt.MigrationsAssembly(assembly));
    })
            .AddDeveloperSigningCredential(); //TO DO : should be using a fixed credential

        #endregion


        #region policies
        services.AddAuthorization(options =>
        {

            options.AddPolicy("SayHi", policy =>
                {
                    policy.RequireClaim("say_hi");
                }
            );
        });
        #endregion
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthRepository, AuthRepository>();

    }
}