using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DevJobsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("Registry")]
        public async Task<IActionResult> Registry(UserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userModel = _mapper.Map<User>(user);
            var newUser = await _authService.RegistrateUser(userModel);
            if (newUser == null)
            {
                return BadRequest();
            }
            return Ok(_mapper.Map<UserDTO>(newUser));
        }

        [HttpPost("login")]
        public async Task<ResponseBase<TokenResponse>> Login(LoginDTO loginDTO)
        {
            var responseTokens = await _authService.Login(loginDTO);
            return responseTokens;
        }

        [HttpPost("RefreshAccessToken")]
        public ResponseBase<TokenResponse> RefreshAccessToken([FromHeader(Name = "RefreshToken")] string refreshToken)
        {
            var responseTokens = _authService.GenerateAccessTokenResponse(refreshToken);
            return responseTokens;
        }

        [HttpPost("ForgotPassword")]
        public async Task<ResponseBase<string>>  ForgotPassword(string email)
        {
           
            var resetPassword = await _authService.ForgotPassword(email);
            
            return resetPassword;
        }
    }
}
