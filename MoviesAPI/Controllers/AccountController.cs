using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : CustomBaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDBContext context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDBContext context, IMapper mapper) : base (context, mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        public async Task<ActionResult<List<UserInfoDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            var queryable = context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            return await Get<ApplicationUser, UserInfoDTO>(pagination);
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo userInfo)
        {
            var user = new ApplicationUser { UserName = userInfo.Email, Email = userInfo.Email };
            var result = await userManager.CreateAsync(user, userInfo.Password);
            if (result.Succeeded)
            {
                return await BuildToken(userInfo, new List<string>());
            }
            else
            {
                return BadRequest("Username or passwrod invalid");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            var result = await signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var usuario = await userManager.FindByEmailAsync(userInfo.Email);
                var roles = await userManager.GetRolesAsync(usuario);
                return await BuildToken(userInfo, roles);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("addUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="Admin")]
        public async Task<ActionResult> AddUserRole(UserRoleCreateDTO createDTO)
        {
            var user = await userManager.FindByIdAsync(createDTO.UserId);
            if (user == null) return NotFound();

            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, createDTO.RoleName));
            return NoContent();
        }

        [HttpPost("removeUserRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveUserRole(UserRoleCreateDTO createDTO)
        {
            var user = await userManager.FindByIdAsync(createDTO.UserId);
            if (user == null) return NotFound();

            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, createDTO.RoleName));
            return NoContent();
        }

        private async Task<UserToken> BuildToken(UserInfo userInfo, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("miValor", "Lo que yo quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var identityUser = await userManager.FindByEmailAsync(userInfo.Email);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));

            var claimsDB = await userManager.GetClaimsAsync(identityUser);

            claims.AddRange(claimsDB);

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddYears(1);

            JwtSecurityToken token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);


            return new UserToken { Expiration = DateTime.Now.AddYears(1), Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}
