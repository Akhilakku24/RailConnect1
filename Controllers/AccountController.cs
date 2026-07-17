using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RailwayReservation.DTOs;
using RailwayReservation.Models; 
using RailwayReservation.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RailwayReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null) 
                return BadRequest(new { message = "Username is already taken!" });

            
            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FullName = model.FullName 
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (!result.Succeeded) 
                return BadRequest(new { message = "User creation failed.", errors = result.Errors });

            // Ensure 'Passenger' role exists
            if (!await _roleManager.RoleExistsAsync("Passenger"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Passenger"));
            }
            
            await _userManager.AddToRoleAsync(user, "Passenger");

            try
            {
                var subject = "Welcome to RailConnect";
                var body = $"<p>Hi {user.FullName},</p><p>Thank you for registering at RailConnect. You can now login and book tickets.</p><p>Safe travels!</p>";
                await _emailService.SendEmailAsync(user.Email!, subject, body);
            }
            catch
            {
                
            }

            return Ok(new { Message = "Registration successful! You can now login as a Passenger." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByNameAsync(model.Username);
            
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Identity IDs are strings by default
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new 
                { 
                    token = new JwtSecurityTokenHandler().WriteToken(token), 
                    expiration = token.ValidTo,
                    username = user.UserName,
                    role = userRoles.FirstOrDefault() // Helpful for the frontend to know the role
                });
            }
            return Unauthorized(new { message = "Invalid username or password." });
        }
    }
}