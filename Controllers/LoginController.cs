using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UpdatingProjects.Dtos;
using UpdatingProjects.Models;
using UpdatingProjects.Services;

namespace UpdatingProjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UpdatingDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        
        public LoginController(UpdatingDbContext context, JwtTokenService jwtTokenService)
        {            
            _context = context;
            _jwtTokenService = jwtTokenService;
        }
         
        [HttpPost("register")]
        public IActionResult Register(RegisterDto userToRegister)
        {
            var user = new UserModel
            { 
                Name = userToRegister.Name,
                Password = userToRegister.Password
            };
            try
            {               
                user.Password = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password);
                _context.LoginData.Add(user);
                _context.SaveChanges();

                return StatusCode(StatusCodes.Status201Created); ;
            } 
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }

        [HttpPost("login")]
        public IActionResult Login (RegisterDto userToLogin)
        {
            try 
            { 
                var user = _context.LoginData.FirstOrDefault(u => u.Name == userToLogin.Name);
            
                    if (user == null || !BCrypt.Net.BCrypt.Verify(userToLogin.Password, user.Password))
                    {
                    return StatusCode(StatusCodes.Status400BadRequest);
                    }

                var jwtToken = _jwtTokenService.GenerateToken(user.Id);
                Response.Cookies.Append("jwtToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true
                });

                return StatusCode(StatusCodes.Status200OK, new { message = "Logged in successfully"});
            }
            catch(Exception )
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = "Bad request" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");
            return StatusCode(StatusCodes.Status200OK, new { message = "Logged out successfully" });
        }

        [HttpGet("user")]
        public IActionResult UserData()
        {
            try
            {
                var jwt = Request.Cookies["jwtToken"];
                var token = _jwtTokenService.Verify(jwt);
                int userId = int.Parse(token.Issuer);
                var user = _context.LoginData.FirstOrDefault(u => u.Id == userId);            
                
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
        }
    }
}
