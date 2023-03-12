using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedeSocial_infnet.Service.ViewModel
{
    public class EdicaoUsuarioViewModel 
    {
        public string userName { get; set; }
        public string Email { get; set; }       
        public string Localidade { get; set; }
        public string AreaMigracao { get; set; }      
    }
}
