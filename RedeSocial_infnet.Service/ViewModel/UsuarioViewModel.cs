using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Service.ViewModel
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage ="Preenchimento do username é obrigatório")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Preenchimento da senha é obrigatório")]
        [MinLength(6, ErrorMessage ="A senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }
        [Required (ErrorMessage ="Preenchimento do e-mail é obrigatório")]
        public string Email { get; set; }       
        public string Localidade { get; set; }
        public string AreaMigracao { get; set; }      
    }
}
