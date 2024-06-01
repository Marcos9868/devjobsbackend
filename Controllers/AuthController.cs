using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
        public async Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO,[CurrentUser] User ?user2)
        {
            var responseTokens = await _authService.Login(loginDTO);
            var userr = user2;
            return responseTokens;
        }

        [HttpPost("RefreshAccessToken")]
        public ResponseModel<TokenResponseModel> RefreshAccessToken([FromHeader(Name = "RefreshToken")] string refreshToken)
        {
            var responseTokens = _authService.GenerateAccessTokenResponse(refreshToken);
            return responseTokens;
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO reset)
        {
            if (!ModelState.IsValid) return BadRequest();
            var resetPassword = await _authService.ResetPassword(reset.Email, reset.NewPassword);
            if (resetPassword == null) return BadRequest();
            return Ok(resetPassword);
        }
    }
}
