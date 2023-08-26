using Application.Features.UserFeatures.RegisterUser;
using Application.Repositories;
using Domain.ViewModels;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthRepository _authRepository;
        public AuthController(IAuthRepository authRepository, IMediator mediator)
        {
            _authRepository = authRepository;
            _mediator = mediator;
        }
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "SayHi")] 
        [Route("[action]")]
        [HttpGet]
        public ActionResult<string> SayHi()
        {
            var x = "Hi";
            return Ok(x);
        }


        //TO DO : token revocation
        [HttpGet]
        [Authorize]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            
            return Ok();
        }
        //for getting refresh token  , use /connect/token endpoint

        // for token revocation , use /connect/revo

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestViewModel model)
        {
            try
            {
                var requestDto = new LoginRequestDTO()
                {
                    ClientId = model.ClientId,
                    ClientSecret = model.ClientSecret,
                    Email = model.Email,
                    Password = model.Password
                };
                LoginResponseDTO signInResult = await _authRepository.SignIn(requestDto, true);
                return Ok(signInResult);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if (ex.InnerException != null)
                    message += " " + ex.InnerException.Message;
                return BadRequest(message);
            }
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<RegisterUserResponseDTO>> Register(RegisterUserRequestViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid is false)
            {
                Dictionary<string, string> errors = new();
                foreach (var entry in ModelState)
                {
                    errors.Add(entry.Key, string.Join(',', entry.Value.Errors.Select(error => error.ErrorMessage)));
                }
                var x = new RegisterUserResponseDTO
                {
                    ErrorMessages = errors
                };
                return BadRequest(x);
            }
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var x = new RegisterUserResponseDTO();
                    x.ErrorMessages.Add("Email", "user is authenticated");
                    return BadRequest(x);
                }
                RegisterUserRequestDTO registerUserRequestDTO = new()
                {
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                var result = await _mediator.Send(registerUserRequestDTO, cancellationToken);
                if (result.Success is false)
                {
                    return BadRequest(result);
                }
                return Ok(result);


            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}
