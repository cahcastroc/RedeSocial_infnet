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


        //[HttpGet]
        //public async Task<ActionResult> Index(string userName)
        //{
        //    PerfilViewModel perfilViewModel = new PerfilViewModel();

        //    using (var httpClient = new HttpClient())
        //    {              
        //        var token = Request.Cookies["jwt"];
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Auth/Perfil/{userName}"))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var apiResponse = await response.Content.ReadAsStringAsync();
        //                if (response.StatusCode == HttpStatusCode.Unauthorized)
        //                {
        //                    return Unauthorized();
        //                }
        //                var usuario = JsonConvert.DeserializeObject<UsuarioViewModel>(apiResponse);

        //                perfilViewModel.Usuario = usuario;


        //            }

        //        }
        //        using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Post/usuario/{userName}"))
        //        {
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var apiResponse = await response.Content.ReadAsStringAsync();
        //                if (response.StatusCode == HttpStatusCode.Unauthorized)
        //                {
        //                    Console.WriteLine("não autorizado no ger userName");
        //                    return Unauthorized();
        //                }
        //                var posts = JsonConvert.DeserializeObject<List<PostViewModel>>(apiResponse);
        //                perfilViewModel.Posts = posts;                      
        //            }                

        //        }
        //        return View(perfilViewModel);
        //    }         



        //}


        [HttpGet]
        public async Task<ActionResult> Index(string userName)
        {
            var perfilViewModel = new PerfilViewModel();
            var token = Request.Cookies["jwt"];

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
