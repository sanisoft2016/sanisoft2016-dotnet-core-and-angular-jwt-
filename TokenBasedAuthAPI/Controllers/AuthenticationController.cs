using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TokenBasedAuthAPI.Models;

namespace TokenBasedAuthAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }

        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [Route("ItemList")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("GetUsers")]
        public IEnumerable<RegisterViewModel> GetUsers()
        {

            var userList =  _userManager.Users.ToListAsync().Result.Select(v=> new RegisterViewModel
            {
                Email=v.Email,
                FirstName=v.FirstName,
                SurName=v.SurName,
                UserName=v.UserName,
                PhoneNumber=v.PhoneNumber
            }).ToList();
            return userList;
        }
        [HttpGet]
        [Route("GetAdminUsers")]
        public IEnumerable<ApplicationUser> GetAdminUsers()
        {
            var userList = _userManager.GetUsersInRoleAsync("Admin");
            return userList.Result;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var isUserExist = await _userManager.FindByNameAsync(register.UserName);
                if (isUserExist != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exist." });

                var user = new ApplicationUser
                {
                    UserCategory = register.UserCategory,
                    Email = register.Email,
                    SecurityStamp = new Guid().ToString(),
                    UserName = register.UserName,
                    FirstName = register.FirstName,
                    SurName = register.SurName,
                    EmailConfirmed = true,
                    PhoneNumber = register.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                var returnString = "";
                if (!result.Succeeded)
                {
                    foreach (var each in result.Errors)
                    {
                        returnString = returnString == "" ? each.Description : returnString + "\n" + each.Description;
                    }
                   var resposeString = StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = returnString });
                    return resposeString;
                }
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                if (register.UserCategory == UserCategory.Admin)
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                else
                    await _userManager.AddToRoleAsync(user, UserRoles.User);

                return Ok(new Response { Status = "Success", Message = "User Created Successfully." });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "One or more required properti(es) were not supplied." });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterViewModel register)
        {
            if (ModelState.IsValid)
            {
                var isUserExist = await _userManager.FindByNameAsync(register.UserName);
                if (isUserExist != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exist." });

                var user = new ApplicationUser
                {
                    Email = register.Email,
                    SecurityStamp = new Guid().ToString(),
                    UserName = register.UserName,
                    FirstName = register.FirstName,
                    SurName = register.SurName,
                    EmailConfirmed = true,
                    PhoneNumber = register.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Unable to create the user" });

                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);

                return Ok(new Response { Status = "Success", Message = "User Created Successfully." });
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "One or more required properti(es) were not supplied." });
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(login.UserName);
                if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    var rolesInString = string.Join(',', userRoles);
                    foreach (var eachRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, eachRole));
                    }
                    var secret = _configuration["JWT:Secret"];
                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        User = user.UserName,
                        Roles = rolesInString
                    });
                }
                return Unauthorized();
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "One or more required properti(es) were not supplied." });
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
               var roleList = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roleList);
                await _userManager.DeleteAsync(user);
                return Ok(new { Status="Sucess", Message="Record deleted successfully" });
            }

            return NotFound();
        }
    }
}
