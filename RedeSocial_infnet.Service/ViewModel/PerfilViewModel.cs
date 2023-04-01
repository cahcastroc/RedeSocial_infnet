using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Service.ViewModel
{
    public class PerfilViewModel
    {
        public UsuarioViewModel Usuario;
        public List<PostViewModel> Posts { get; set; }
        private readonly HttpContext _httpContext;

        public PerfilViewModel(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public string UsuarioLogado => _httpContext.Session.GetString("user");
    }
}