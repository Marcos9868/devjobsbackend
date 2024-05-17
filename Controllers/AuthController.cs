using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
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
        public IActionResult Registry(UserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userModel = _mapper.Map<User>(user);
            var newUser = _authService.RegistrateUser(userModel);
            if (newUser == null)
            {
                return BadRequest();
            }
            return Ok(_mapper.Map<UserDTO>(newUser));
        }
        [HttpPost("login")]
         public async Task<ResponseModel<TokenResponseModel>> Login(LoginDTO loginDTO)
        {
            var responseTokens = await _authService.Login(loginDTO);
            return responseTokens;
        }
        [HttpPost("RefreshAccessToken")]
         public  ResponseModel<TokenResponseModel> RefreshAccessToken(string RefreshToken)
        {
            var responseTokens =  _authService.GenerateAccessTokenWithResponse(RefreshToken);


            return responseTokens;
        }
    }
}
