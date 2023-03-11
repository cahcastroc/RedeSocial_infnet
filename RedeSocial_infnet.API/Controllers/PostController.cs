using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedeSocial_infnet.Domain.Models;
using RedeSocial_infnet.Service.Data;
using RedeSocial_infnet.Service.ViewModel;

namespace RedeSocial_infnet.API.Controllers
{
  
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
            Console.WriteLine("entrou aqui");
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


        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, PostViewModel postViewModel)
        {
            Post post = await _context.Posts.FindAsync(id);

            if (id != post.Id)
            {
                return BadRequest();
            }

            //if(post.UserName != User.Identity.Name)
            //{
            //    return Unauthorized();
            //}

            post.Titulo = postViewModel.Titulo;
            post.Conteudo = postViewModel.Conteudo;
            post.UserName = postViewModel.UserName;
            post.EditadoEm = DateTime.Now;


            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<PostViewModel>> PostPost(PostViewModel postViewModel)
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
              


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            Post post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            //if(post.UserName != User.Identity.Name)
            //{
            //    return Unauthorized();
            //}

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
