using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RedeSocial_infnet.Service.ViewModel;
using System.Net.Http;

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
        public IActionResult Login()
        {         
            return View();
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
