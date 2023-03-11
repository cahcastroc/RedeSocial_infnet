using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Service.ViewModel
{
   public  class LoginViewModel
    {
        [Required(ErrorMessage ="Preenchimento obrigatório")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Preenchimento obrigatório")]
        public string Password { get; set; }
    }
}
