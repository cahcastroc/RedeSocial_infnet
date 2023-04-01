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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace RedeSocial_infnet.MVC.Controllers
{

    public class PostController : Controller
    {


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");
            var user = HttpContext.Session.GetString("user");

            ViewBag.userName = user;

            List<Post> postagens = new List<Post>();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);


                using (var response = await client.GetAsync("https://localhost:5001/api/post"))
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

            var jwtToken = HttpContext.Session.GetString("JwtToken");
            if (jwtToken == null)
            {
                return RedirectToAction("Erro401", "Home");
            }

            return View();
        }

        public static async Task<byte[]> FormFileToByteArrayAsync(IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [HttpPost]
        public async Task<IActionResult> NovoPost(string titulo, string conteudo, IFormFile imagem)
        {
            PostViewModel postViewModel = new();
            postViewModel.Titulo = titulo;
            postViewModel.Conteudo = conteudo;
            byte[] blobBytes = await FormFileToByteArrayAsync(imagem);
            postViewModel.Imagem = blobBytes;

            var jwtToken = HttpContext.Session.GetString("JwtToken");

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                StringContent content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");


                using (var response = await httpClient.PostAsync("https://localhost:5001/api/Post", content))
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
            var jwtToken = HttpContext.Session.GetString("JwtToken");
            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                using (var response = await httpClient.GetAsync($"https://localhost:5001/api/Post/usuario/{userName}"))
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

        [HttpPost]
        public async Task<IActionResult> ExcluirPost(int id)
        {
            var jwtToken = HttpContext.Session.GetString("JwtToken");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                using (var response = await httpClient.DeleteAsync($"https://localhost:5001/api/Post/{id}"))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Erro401", "Home");
                    }

                    return RedirectToAction("Index", "PostsUsuario");
                }
            }
        }

    }


}
