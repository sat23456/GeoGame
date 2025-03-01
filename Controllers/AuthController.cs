using GeoGame.Models;
using GeoGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public AuthController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterRequest request)
        {
            // Call the Register method in the MongoDbService
            bool isRegistered = _mongoDbService.Register(request.Email, request.Password);

            if (isRegistered)
            {
                // Return success message
                return Ok("User registered successfully.");
            }
            else
            {
                // Return error message if email already exists
                return BadRequest("Email already exists.");
            }
        }

        // POST api/user/login
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            // Call the Login method in MongoDbService
            bool isAuthenticated = _mongoDbService.Login(request.Email, request.Password);

            if (isAuthenticated)
            {
                // Return success message if credentials are correct
                return Ok("Login successful.");
            }
            else
            {
                // Return error message if credentials are incorrect
                return Unauthorized("Invalid email or password.");
            }
        }
    }

    // Request model for Register
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Request model for Login
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
