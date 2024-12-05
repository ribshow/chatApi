using Microsoft.AspNetCore.Mvc;
using chatApi.Models;
using chatApi.Responses;
using MongoDB.Driver;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace chatApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ContextMongoDb _context;

        public AuthController(ContextMongoDb context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a valid JWT token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth
        ///     {
        ///         "email": "test@example.com",
        ///         "password": "test1234!"
        ///     }
        /// </remarks>
        /// <response code="200">User authenticated successully</response>
        /// <response code="401">Unauthorized - Credentials invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] Users user)
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();

            foreach (Users userEmail in users)
            {
                if (user.Email == userEmail.Email && user.Password == userEmail.Password)
                {
                    var token = GenerateJwtToken(user.Email);

                    return Ok(new AuthResponse {message = "User authenticated successfully", token =  token });
                }
            }
            return Unauthorized();
        }

        /// <summary>
        /// Register a user 
        /// </summary>
        /// <param name="user"></param>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/auth/register
        ///     {
        ///         "email": "test@example.com",
        ///         "password": "test1234!"
        ///     }
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">User Created with successfully</response>
        /// <response code="400">Generally Email or Password invalid</response>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] Users user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            Users newUser = new(user.Email, user.Password);

            await _context.Users.InsertOneAsync(newUser);
            return Ok(new {Message = "User created with successfully"});

        }

        private string GenerateJwtToken(string nickname)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nickname),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, "http://127.0.0.1:8000"),
                new Claim(JwtRegisteredClaimNames.Iss, "https://localhost:7125")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVESUPERSEGURACHAVESUPERSEGURA"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7125",
                audience: "http://127.0.0.1:8000",
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
