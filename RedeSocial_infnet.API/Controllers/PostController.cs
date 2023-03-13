using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.Data;
using RedeSocial_infnet.Service.ViewModel;

namespace RedeSocial_infnet.API.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostController(AppDbContext context)
        {
            _context = context;
        }
     

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
                 
                return await _context.Posts.ToListAsync();           
            
        }

       
        [HttpGet("usuario/{userName}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsUser(string userName)
        {
          
            return await _context.Posts.Where(p => p.UserName == userName).ToListAsync();           
         
                  
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<PostViewModel>> GetPost(int id)
        {
         
                var post = await _context.Posts.FindAsync(id);

                if (post == null)
                {
                    return NotFound();
                }

                PostViewModel postViewModel = new PostViewModel();
                postViewModel.UserName = post.UserName;
                postViewModel.Titulo = post.Titulo;
                postViewModel.Conteudo = post.Conteudo;
                postViewModel.CriadoEm = post.CriadoEm;
                return postViewModel;           
        }
              
        
       
        [HttpPost]
        public async Task<ActionResult<PostViewModel>> NovoPost(PostViewModel postViewModel)
        {
            postViewModel.UserName = User.Identity.Name;

            Post post = new Post();

            post.UserName = postViewModel.UserName;          
            post.CriadoEm = DateTime.Now;
            post.Titulo = postViewModel.Titulo;
            post.Conteudo = postViewModel.Conteudo;

          
        
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, postViewModel);
        }                    

      
    }
}
