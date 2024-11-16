using Microsoft.AspNetCore.Mvc;
using chatApi.Models;
using chatApi.Services;
using MongoDB.Driver;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

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
        
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Users user)
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();

            foreach(Users userEmail in users)
            {
                if(user.Email == userEmail.Email)
                {
                    var token = GenerateJwtToken(user.Email);
          
                    return Ok(new { token });
                }
            }
            return Unauthorized();
        }

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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVESUPERSEGURACHAVESUPERSEGURA"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "127.0.0.1:8000",
                audience: "127.0.0.1:8000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
