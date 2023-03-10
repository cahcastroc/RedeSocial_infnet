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

        // GET: api/Post
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

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, PostViewModel postViewModel)
        {
            Post post = await _context.Posts.FindAsync(id);

            if (id != post.Id)
            {
                return BadRequest();
            }

            if(post.UserName != User.Identity.Name)
            {
                return Unauthorized();
            }

            post.Titulo = postViewModel.Titulo;
            post.Conteudo = postViewModel.Conteudo;
            post.UserName = User.Identity.Name;
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
        public async Task<ActionResult<Post>> PostPost(PostViewModel postViewModel)
        {
            Post post = new Post();
            post.UserName = User.Identity.Name;
            post.CriadoEm = DateTime.Now;
            post.Titulo = postViewModel.Titulo;
            post.Conteudo = postViewModel.Conteudo;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            Post post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if(post.UserName != User.Identity.Name)
            {
                return Unauthorized();
            }

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
