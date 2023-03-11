using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.ViewModel;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace RedeSocial_infnet.MVC.Controllers
{
    public class PostController : Controller
    {
        [Route("feed")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            List<PostViewModel> postagens = new List<PostViewModel>();

            using (var client = new HttpClient())
            {
               using (var response = await client.GetAsync("https://localhost:7098/api/post"))
                {
                   string apiResponse = await response.Content.ReadAsStringAsync();
                   postagens = JsonConvert.DeserializeObject<List<PostViewModel>>(apiResponse);
               }
            }


            return View(postagens);
        }

        public ViewResult NovoPost() => View();


        [HttpPost]
        public async Task<IActionResult> NovoPost(PostViewModel postViewModel)
        {
                     


            using (var httpClient = new HttpClient())
            {                            
              
                var token = Request.Cookies["jwt"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                StringContent content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");


                using (var response = await httpClient.PostAsync("https://localhost:7098/api/Post", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    postViewModel = JsonConvert.DeserializeObject<PostViewModel>(apiResponse);
                }
            }
            return View(postViewModel);
        }


        [HttpGet]
        [Route("posts/usuario/{userName}")]
        public async Task<IActionResult> PostsUsuario(string userName)
        {
        
            using (var httpClient = new HttpClient())
            {
                var token = Request.Cookies["jwt"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using (var response = await httpClient.GetAsync($"https://localhost:7098/api/Post/usuario/{userName}"))
                {
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

    //public ViewResult Create() => View();

    

}
