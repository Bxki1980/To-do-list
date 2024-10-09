using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_do_list.Models;
using To_do_list.Repositories;

namespace To_do_list.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginRepository _loginRepository;

        public LoginController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        // POST: api/Login/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Login([FromBody] AuthenticateModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid client request");
            }

            var token = await _loginRepository.AuthenticateAsync(user.UserName, user.Password);
            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new { Token = token });
        }

        // POST: api/Login/CreateNewAccount
        [AllowAnonymous]
        [HttpPost("CreateNewAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] UserModel user)
        {
            try
            {
                await _loginRepository.CreateAccountAsync(user);
                return Ok("Account created successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Failed to create account. Please try again later.");
            }
        }

        // GET: api/Login/GetAllUsers
        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var allUsers = await _loginRepository.GetAllUsersAsync();
                return Ok(allUsers);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Login/DeleteUser/5
        [Authorize]
        [HttpDelete("DeleteUser/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _loginRepository.DeleteAccountAsync(id);
                if (result)
                {
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return NotFound("User not found.");
                }
            }
            catch (Exception)
            {
                return BadRequest("An error occurred while deleting the user.");
            }
        }

        // PUT: api/Login/UpdateUser/5
        [Authorize]
        [HttpPut("UpdateUser/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel user)
        {
            try
            {
                var result = await _loginRepository.UpdateUserAsync(id, user);
                if (result)
                {
                    return Ok("User updated successfully.");
                }
                else
                {
                    return NotFound("User not found.");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
