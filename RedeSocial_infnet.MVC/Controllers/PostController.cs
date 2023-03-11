using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.ViewModel;
using System.Text;

namespace RedeSocial_infnet.MVC.Controllers
{
    public class PostController : Controller
    {
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

            if (User.Identity.IsAuthenticated)
            {
                Console.WriteLine("Usuário logado" + User.Identity.Name);
            }
            else {
                Console.WriteLine("Não tá logado");
            }


            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(postViewModel), Encoding.UTF8, "application/json");


                using (var response = await httpClient.PostAsync("https://localhost:7098/api/Post", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    postViewModel = JsonConvert.DeserializeObject<PostViewModel>(apiResponse);
                }
            }
            return View(postViewModel);
        }

    }

    //public ViewResult Create() => View();

    

}
