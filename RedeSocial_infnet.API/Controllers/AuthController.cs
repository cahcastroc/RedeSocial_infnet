using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.Jwt;
using RedeSocial_infnet.Service.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RedeSocial_infnet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly JwtConfig jwtBearerTokenConfig;
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;

        public AuthController(IOptions<JwtConfig> jwtTokenOptions, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            this.jwtBearerTokenConfig = jwtTokenOptions.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpPost]
        [Route("Registro")]
        public async Task<IActionResult> Registro([FromBody] UsuarioViewModel user)
        {
            if (!ModelState.IsValid || user == null)
            {
                return new BadRequestObjectResult(new { Message = "Falha ao registrar o usuário." });
            }

            Usuario usuario = new Usuario
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                Localidade = user.Localidade,
                AreaMigracao = user.AreaMigracao
            };

            IdentityUser identityUser = new IdentityUser() { UserName = usuario.UserName, Email = usuario.Email };

           var identityResult = await userManager.CreateAsync(usuario, usuario.Password);
            if (!identityResult.Succeeded)
            {
                ModelStateDictionary dictionary = new ModelStateDictionary();               
                foreach (IdentityError error in identityResult.Errors)
                {
                    dictionary.AddModelError(error.Code, error.Description);
                }

                return new BadRequestObjectResult(new { Message = "Erro no registro do usuário.", Errors = dictionary });
            }

            return Ok(new { Message = "Registro realizado com sucesso" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Login credenciais)
        {
            IdentityUser identityUser;

            if (!ModelState.IsValid
                || credenciais == null
                || (identityUser = await ValidateUser(credenciais)) == null)
            {
                return new BadRequestObjectResult(new { Message = "Falha no Login" });
            }

            var token = GenerateToken(identityUser);
            return Ok(new { Token = token, Message = "Login efetuado com Sucesso" });
        }

        private async Task<Usuario> ValidateUser(Login credenciais)
        {
           var identityUser = await userManager.FindByNameAsync(credenciais.UserName);
            if (identityUser != null)
            {
                PasswordVerificationResult result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credenciais.Password);
                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }

        private object GenerateToken(IdentityUser identityUser)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(jwtBearerTokenConfig.SecretKey);
            
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, identityUser.Email)
                }),

                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenConfig.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenConfig.Audience,
                Issuer = jwtBearerTokenConfig.Issuer
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
