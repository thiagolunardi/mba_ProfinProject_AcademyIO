using AcademyIO.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AcademyIO.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AcademyIO.API.ViewModel.UserViewModel;
using System.Net;
using MediatR;
using AcademyIO.ManagementStudents.Application.Commands;

namespace AcademyIO.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController(SignInManager<IdentityUser<Guid>> _signInManager,
                                UserManager<IdentityUser<Guid>> _userManager,
                                JwtSettings _jwtSettings,
                                IMediator _mediator,
                                INotifier notifier) : MainController(notifier)
    {
        /// <summary>
        /// Registra novo usuário que pode ser um estudante ou um administrador
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Retorna os dados do usuário crido</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await RegisterUser(model, model.IsAdmin ? "ADMIN" : "STUDENT");

            var command = new AddUserCommand(model.Email, model.IsAdmin, model.FirstName, model.LastName, model.DateOfBirth, model.Email);
            await _mediator.Send(command);

            var token = await GetJwt(model.Email!);
            return CustomResponse(model);
        }

        /// <summary>
        /// Recebe os dados do usuário para login
        /// </summary>
        /// <param name="loginUser"></param>
        /// <returns>Se os dados forem válidos, retorna dados do usuário logado com token</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

            if (result.Succeeded)
                return CustomResponse(await GetJwt(loginUser.Email));

            if (result.IsLockedOut)
            {
                NotifieError("Este usuário está temporariamente bloqueado");
                return CustomResponse(loginUser);
            }

            NotifieError("Usuário ou senha incorretos");
            return CustomResponse(loginUser);
        }

        private async Task<LoginResponseViewModel> GetJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var userClaims = await _userManager.GetClaimsAsync(user);

            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                userClaims.Add(new Claim(ClaimTypes.Role, role));

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(userClaims);

            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var handler = new JwtSecurityTokenHandler();
            var signingConf = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = signingConf,
                Subject = identityClaims,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(_jwtSettings.ExpirationHours)
            });

            var encodedJwt = handler.WriteToken(securityToken);

            var response = new LoginResponseViewModel
            {
                AccessToken = encodedJwt,
                ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpirationHours).TotalSeconds,
                UserToken = new UserTokenViewModel
                {
                    Id = user.Id.ToString(),
                    Email = user.Email,
                    Claims = userClaims.Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value })
                }
            };

            return response;
        }

        private async Task<(IdentityResult IdentityResult, string UsuarioId)> RegisterUser(RegisterUserViewModel registerUser, string role)
        {
            var userIdentity = new IdentityUser<Guid>
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(userIdentity, registerUser.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(role))
                    await _userManager.AddToRoleAsync(userIdentity, role);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    NotifieError(error.Description);
                }
            }

            return (result, result.Succeeded ? userIdentity.UserName : string.Empty);
        }
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}