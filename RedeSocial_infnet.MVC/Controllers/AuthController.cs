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

        [HttpGet]
        public ViewResult Login() => View();
    

        [HttpGet]
        public ViewResult Cadastro() => View();      



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastro(UsuarioViewModel model)
        {            

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("https://localhost:5001/api/Auth/Cadastro", model);
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

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync("https://localhost:5001/api/Auth/Login", model);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();

                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("user", model.UserName);              

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

            var jwtToken = HttpContext.Session.GetString("JwtToken");
            var user = HttpContext.Session.GetString("user");


            if (userName != user)
            {
                return RedirectToAction("Erro401", "Home");
            }

            EdicaoUsuarioViewModel usuarioEdicao = new EdicaoUsuarioViewModel();
            using (var httpClient = new HttpClient())
            {
                UsuarioViewModel usuario = new UsuarioViewModel();
               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                using (var response = await httpClient.GetAsync($"https://localhost:5001/api/Auth/Perfil/{userName}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var usuarioResponse = JsonConvert.DeserializeObject<UsuarioViewModel>(apiResponse);
                        usuario = usuarioResponse;
                        usuarioEdicao.userName = usuario.UserName;
                        usuarioEdicao.Email = usuario.Email;
                        usuarioEdicao.Localidade = usuario.Localidade;
                        usuarioEdicao.AreaMigracao = usuario.AreaMigracao;
                    }                  
                }
              
            }
          
            return View(usuarioEdicao);
        }


        //Put edição usuário na API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EdicaoUsuarioViewModel model)
        {
                      
            using (var client = new HttpClient())
            {
                var jwtToken = HttpContext.Session.GetString("JwtToken");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                using (var resposta = await client.PutAsync($"https://localhost:5001/api/auth/editar/{model.userName}", content))
                {
                    if (resposta.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }
                    if (resposta.IsSuccessStatusCode) {
                        ViewBag.Sucesso = "Edição realizada com sucesso";
                        return RedirectToAction("Index", "Post");
                    }
                }
            }

            return View(model);
        }

        public IActionResult Logout (){ 
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Remove("user");
            HttpContext.Session.Clear();            
            return RedirectToAction("Index", "Home");

        }



    }
}
