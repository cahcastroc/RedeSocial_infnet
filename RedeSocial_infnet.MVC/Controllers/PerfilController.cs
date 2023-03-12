using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RedeSocial_infnet.Service.ViewModel;
using System.Net;
using System.Net.Http.Headers;

namespace RedeSocial_infnet.MVC.Controllers
{
    public class PerfilController : Controller
    {

        [HttpGet]
        public async Task<ActionResult> Index(string userName)
        {
            var perfilViewModel = new PerfilViewModel();
            var token = Request.Cookies["jwt"];
            var user = Request.Cookies["user"];

            ViewBag.userName = user;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
           
                using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Auth/Perfil/{userName}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }

                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<UsuarioViewModel>(apiResponse);
                    perfilViewModel.Usuario = usuario;
                }

                // Obter postagens do usuário
                using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Post/usuario/{userName}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }

                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<PostViewModel>>(apiResponse);
                    perfilViewModel.Posts = posts;
                }
            }
            return View(perfilViewModel);
        }



    }
}
