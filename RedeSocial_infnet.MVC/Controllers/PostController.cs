using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.ViewModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;


namespace RedeSocial_infnet.MVC.Controllers
{

    public class PostController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string jwtToken;

        public PostController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            jwtToken = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];          
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Post> postagens = new List<Post>();


            using (var client = new HttpClient())            {
            
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
        public IActionResult NovoPost() {

            if (jwtToken == null) {
                return RedirectToAction("Erro401", "Home");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> NovoPost(PostViewModel postViewModel)
        {
        

            using (var httpClient = new HttpClient())
            {

               

                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

            using (var httpClient = new HttpClient())
            {
                var token = Request.Cookies["jwt"];
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

    //public ViewResult Create() => View();



}
