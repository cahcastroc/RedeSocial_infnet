using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedeSocial_infnet.MVC.Models;
using RedeSocial_infnet.Service.ViewModel;


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
        public async Task<IActionResult> Login()
        {         
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cadastro()
        {
            return View();
        }


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


    }
}
