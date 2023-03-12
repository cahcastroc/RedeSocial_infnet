using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedeSocial_infnet.MVC.Models;
using RedeSocial_infnet.Service.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RedeSocial_infnet.MVC.Controllers
{
    public class AuthController : Controller
    {

        private readonly IHttpClientFactory _clientFactory;

        public AuthController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }


        [HttpGet]
        public ViewResult Login() => View();
    

        [HttpGet]
        public ViewResult Cadastro() => View();

        //[HttpGet]
        //public ViewResult Editar() => View();



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(UsuarioViewModel model)
        {
            

            var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("https://localhost:7098/api/Auth/Cadastro", model);
            if (response.IsSuccessStatusCode)
            {               
                return RedirectToAction("Login", "Auth");
            }
            else { 
                ModelState.AddModelError("", "Erro ao realizar o cadastro. Verifique os dados inseridos e tente novamente");
                return View(model);
            
            }
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
   
            var httpClient = _clientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("https://localhost:7098/api/Auth/Login", model);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

           
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTimeOffset.Now.AddMinutes(10)
                });                          

                return RedirectToAction("Index", "Post");
            }
            else
            {
                
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
        }


        public async Task<ActionResult> Editar(string userName)
        {

            var token = Request.Cookies["jwt"];
            //var handler = new JwtSecurityTokenHandler();
            //var tokenS = handler.ReadJwtToken(token);
            //var name = tokenS.Claims.First(c => c.Type == "userName").Value;

            //if(userName != name)
            //{
            //    return RedirectToAction("Erro401", "Home");
            //}

            EdicaoUsuarioViewModel usuarioEdicao = new EdicaoUsuarioViewModel();
            using (var httpClient = new HttpClient())
            {
                UsuarioViewModel usuario = new UsuarioViewModel();
               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Auth/Perfil/{userName}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }

                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var usuarioResponse = JsonConvert.DeserializeObject<UsuarioViewModel>(apiResponse);
                    usuario = usuarioResponse;

                }
                usuarioEdicao.userName = usuario.UserName;
                usuarioEdicao.Email = usuario.Email;
                usuarioEdicao.Localidade = usuario.Localidade;
                usuarioEdicao.AreaMigracao = usuario.AreaMigracao;
            }
            Console.WriteLine("nome do usuário" + usuarioEdicao.userName);
            return View(usuarioEdicao);
        }



        //[HttpPut]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Editar(EdicaoUsuarioViewModel model)
        //{
        //    using (var client = new HttpClient())
        //    {

        //        var token = Request.Cookies["jwt"];
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");


        //        using (var resposta = await client.PutAsync("https://localhost:7098/api/Post", content))
        //        {
        //            string apiResponse = await resposta.Content.ReadAsStringAsync();
        //            model = JsonConvert.DeserializeObject<EdicaoUsuarioViewModel>(apiResponse);
        //        }
        //    }

        //    return View(model);                       

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EdicaoUsuarioViewModel model)
        {
                      
            using (var client = new HttpClient())
            {
                var token = Request.Cookies["jwt"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using (var resposta = await client.PutAsync($"https://localhost:7098/api/auth/editar/{model.userName}", content))
                {
                    if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }                   
                }
            }

            return View(model);
        }



    }
}
