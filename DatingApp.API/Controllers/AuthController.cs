using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data.Interface;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _congif;
        public AuthController(IAuthRepository repo, IConfiguration congif)
        {
            _repo = repo;
            _congif = congif;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User{
             Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login(UserForLoginDto userForRegisterDto)
        {
                var userFromrepo = await _repo.Login(userForRegisterDto.Username.ToLower(), userForRegisterDto.Password);
                if(userFromrepo == null)
                    return Unauthorized();
                
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromrepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromrepo.Username)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_congif.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescripter = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescripter);

                return Ok(new {
                    token = tokenHandler.WriteToken(token)
                });
         }
    }
}