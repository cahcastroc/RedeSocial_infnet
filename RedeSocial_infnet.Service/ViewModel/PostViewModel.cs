using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RedeSocial_infnet.Service.ViewModel
{
    public class PostViewModel
    {
        public int Id { get; set; }

        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public string UserName { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime EditadoEm { get; set; }
        public byte[] Imagem { get; set; }

    }
}
