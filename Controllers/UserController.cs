using System.Security.Claims;
using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevJobsBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await _userService.GetUsers();
            if (users.Count == 0) return NoContent();
            return Ok(users);
        }
        [Authorize]
        [HttpGet("Me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserSession()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            var userId = userIdClaim.Subject.ToString();
            var userIdConverted = int.Parse(userId);
            var user = await _userService.Me(userIdConverted);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [HttpGet("{idUser}")]
        public async Task<IActionResult> Show(int idUser)
        {
            var user = await _userService.GetUser(idUser);
            if (user is null) return NotFound();
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> AddUser(UserDTO user)
        {
            if (string.IsNullOrEmpty(user.ToString())) throw new Exception("");
            var userModel = _mapper.Map<User>(user);
            var newUser = await _userService.AddUser(userModel);
            if (newUser is null) return BadRequest();
            return Ok(_mapper.Map<UserDTO>(newUser));
        }
        [HttpPut("{idUser}")]
        public async Task<IActionResult> Update(int idUser, UserDTO user)
        {
            if (string.IsNullOrEmpty(user.ToString())) return BadRequest();
            var userModel = _mapper.Map<User>(user);
            userModel.Id = idUser;
            var userToUpdate = await _userService.UpdateUser(userModel);
            if (userToUpdate is null) return BadRequest();
            return Ok(userToUpdate);
        }
        [HttpDelete("{idUser}")]
        public async Task<IActionResult> Remove(int idUser)
        {
            var user = await _userService.GetUser(idUser);
            if (user is null) return NotFound();
            var userToRemove = await _userService.RemoveUser(user);
            if (userToRemove is null) return BadRequest();
            return Ok(userToRemove);
        }

        [HttpPut("ResetPassword")]
        public async Task<ResponseBase<User>> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (string.IsNullOrEmpty(resetPasswordDto.NewPassword) || string.IsNullOrEmpty(resetPasswordDto.JwtToken))
            {
                return new ResponseBase<User>
                {
                    Status = false,
                    Message = "Password and token are required"
                };
            }

            var response = await _userService.ResetPassword(resetPasswordDto.NewPassword, resetPasswordDto.JwtToken);
           
            return response;
        }

    }
}