using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Domain.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime EditadoEm { get; set; }
        public string UserName { get; set;}       
        public byte[] Imagem { get; set; }
   
    }
}
