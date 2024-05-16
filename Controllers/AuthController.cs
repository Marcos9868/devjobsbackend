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
        // [HttpGet("testehash")]
        // public Task<string> TesteHash(){
        //     var testeHashed = _authService.GenerateHashPassword("teste");

        //     return testeHashed;
        // }
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
        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO reset)
        {
            if(!ModelState.IsValid) return BadRequest();
            var resetPassword = await _authService.ResetPassword(reset.Email, reset.NewPassword);
            if (resetPassword == null) return BadRequest();
            return Ok(resetPassword);
        }
    }
}
