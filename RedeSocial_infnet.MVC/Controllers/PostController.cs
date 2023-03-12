using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace RedeSocial_infnet.MVC.Controllers
{

    public class PostController : Controller
    {


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jwtToken = Request.Cookies["jwt"];
            var user = Request.Cookies["user"];

            ViewBag.userName = user;

            List<Post> postagens = new List<Post>();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);


                using (var response = await client.GetAsync("https://localhost:7098/api/post"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    postagens = JsonConvert.DeserializeObject<List<Post>>(apiResponse);


                    return View(postagens);

                }
            }

        }


        [HttpGet]
        public IActionResult NovoPost()
        {

            var jwtToken = Request.Cookies["jwt"];
            if (jwtToken == null)
            {
                return RedirectToAction("Erro401", "Home");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NovoPost(PostViewModel postViewModel)
        {
            var jwtToken = Request.Cookies["jwt"];

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                StringContent content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");


                using (var response = await httpClient.PostAsync("https://localhost:7098/api/Post", content))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    postViewModel = JsonConvert.DeserializeObject<PostViewModel>(apiResponse);
                    return View(postViewModel);
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> PostsUsuario(string userName)
        {
            var jwtToken = Request.Cookies["jwt"];
            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Post/usuario/{userName}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var posts = JsonConvert.DeserializeObject<List<PostViewModel>>(apiResponse);
                        return View(posts);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
        }

    }


}
