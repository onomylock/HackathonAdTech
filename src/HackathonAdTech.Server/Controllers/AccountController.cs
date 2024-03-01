using AutoMapper;
using HackathonAdTech.Domain.Entities.DTO.Auth;
using HackathonAdTech.Domain.Entities.Identity;
using HackathonAdTech.Server.JwtFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace HackathonAdTech.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;        
        private readonly JwtHandler _jwtHandler;

        public AccountController(UserManager<AppUser> userManager, JwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
            _userManager = userManager;           
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthDto userForAuth)
        {
            var user = await _userManager.FindByEmailAsync(userForAuth.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuth.Password))
                return Unauthorized(new AuthResponseDto() { ErrorMessage = "Invalid Authentication" });

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token});
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> Registration([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (await _userManager.FindByEmailAsync(userForRegistration.Email) != null)
                return Conflict(new AuthResponseDto() { ErrorMessage = $"User with email {userForRegistration.Email} already exists" });

            var user = new AppUser() { Email = userForRegistration.Email, UserName = userForRegistration.UserName };

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if(result.Succeeded)
            {
                var signingCredentials = _jwtHandler.GetSigningCredentials();
                var claims = _jwtHandler.GetClaims(user);
                var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return Ok(new AuthResponseDto() { IsAuthSuccessful = true, Token = token});
            }

            throw new Exception($"User creation failed. Errors:\n {string.Join("\n ", result.Errors.Select(ex => ex.Description))}");
        }
    }
}
