using BankingAPI.Interfaces;
using BankingAPI.Misc;
using BankingAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace BankingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService,ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public  ActionResult<RegisterUserResponse> RegisterUser(RegisterUserRequest request)
        {
            try
            {
                var result =  _authenticationService.Register(request);
                return Ok(result);
            }
            catch(UnableToCreateEntityException ex)
            {
                _logger.LogWarning($"Unable to register user check the Email {request.Email} and phone {request.Phone}");

                return BadRequest(ex.Message);
            }
            catch (Exception ex) 
            { 
                _logger.LogWarning("Unable to register user check the Email and phone ");
                return BadRequest(ex.Message); 
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> CustomerLogin(LoginRequest request)
        {
            try
            {
                var result =  _authenticationService.Login(request);
                return Ok(result);
            }
            catch (InvalidCredentialException ex)
            {
                _logger.LogDebug("login attempt Fail for username: {Username}. Reason: {Message}", request.Username, ex.Message);
                 _logger.LogError("Failed login attempt ");

                _logger.LogWarning("Failed login attempt for username: {Username}. Reason: {Message}", request.Username, ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}