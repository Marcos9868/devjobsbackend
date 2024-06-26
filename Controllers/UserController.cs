using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
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
    }
}