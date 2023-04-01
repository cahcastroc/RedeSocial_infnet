using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Domain.Models
{
    public class Usuario : IdentityUser
    {

        [Required]
        public string Password { get; set; }

        public string Localidade { get; set; }
        public string AreaMigracao { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime EditadoEm { get; set; }
        public byte[] FotoPerfil { get; set; }


    }
}
