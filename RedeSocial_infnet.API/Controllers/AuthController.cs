using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("perfil/{userName}")]
        public async Task<ActionResult<UsuarioViewModel>> Perfil(string userName)
        {

            Usuario usuario = await userManager.FindByNameAsync(userName);

            if (usuario == null)
            {
                return NotFound();
            }

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel
            {
                UserName = usuario.UserName,
                Email = usuario.Email,
                Localidade = usuario.Localidade,
                AreaMigracao = usuario.AreaMigracao,
            };

            return Ok(usuarioViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Cadastro")]
        public async Task<IActionResult> Cadastro([FromBody] UsuarioViewModel user)
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
                AreaMigracao = user.AreaMigracao,
                CriadoEm = DateTime.Now
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

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            Console.WriteLine("entrou no login");
            Login credenciais = new Login();
            credenciais.UserName = loginViewModel.UserName;
            credenciais.Password = loginViewModel.Password;

            Usuario usuario;

            if (!ModelState.IsValid
                || credenciais == null
                || (usuario = await ValidateUser(credenciais)) == null)
            {
                Console.WriteLine("Login inválido");
                return new BadRequestObjectResult(new { Message = "Falha no Login" });
            }

            var token = GenerateToken(usuario);
            Console.WriteLine("Token gerado");
            return Ok(token);
        }

        [Authorize]
        [HttpPut]
        [Route("Editar/{userName}")]
        public async Task<IActionResult> Editar(string userName, [FromBody] EdicaoUsuarioViewModel usuarioAtualizado)
        {

            if (usuarioAtualizado == null)
            {
                return new BadRequestObjectResult(new { Message = "Falha ao editar o usuário." });
            }

            if (userName != User.Identity.Name)
            {
                return new UnauthorizedObjectResult(new { Message = "Você não tem permissão para editar este usuário." });
            }

            Usuario usuarioAtual = await userManager.FindByNameAsync(userName);

            if (usuarioAtual == null)
            {
                Console.WriteLine(" null");
                return NotFound();
            }
            usuarioAtual.UserName = usuarioAtualizado.userName;
            usuarioAtual.Email = usuarioAtualizado.Email;
            usuarioAtual.Localidade = usuarioAtualizado.Localidade;
            usuarioAtual.AreaMigracao = usuarioAtualizado.AreaMigracao;
            usuarioAtual.EditadoEm = DateTime.Now;

            IdentityResult resultado = await userManager.UpdateAsync(usuarioAtual);

            if (resultado.Succeeded)
            {
                return Ok(new { Message = "Usuário editado com sucesso" });
            }
            else
            {
                return BadRequest(new { Message = "Falha ao editar o usuário." });
            }
        }
        private async Task<Usuario> ValidateUser(Login credenciais)
        {
            Usuario identityUser = await userManager.FindByNameAsync(credenciais.UserName);
            if (identityUser != null)
            {
                PasswordVerificationResult result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credenciais.Password);

                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }

        private object GenerateToken(Usuario usuario)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(jwtBearerTokenConfig.SecretKey);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email)

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
