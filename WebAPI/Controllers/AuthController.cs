using Application.Features.UserFeatures.RegisterUser;
using Application.Repositories;
using Domain.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize(Roles = "Admin")]
        //[Authorize(Policy = "SayHi")] TODO : policies do not apply
        [Route("[action]")]
        [HttpGet]
        public ActionResult<string> SayHi()
        {
            var x = "Hi";
            return Ok(x);
        }

        //TO DO : token revocation
        
        
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO credentials)
        {
            try
            {
                LoginResponseDTO signInResult = await _authRepository.SignIn(credentials, true);
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
