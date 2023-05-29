using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class Login2FADto
	{
        public bool rememberMachine { get; set; }
        public bool rememberAccount { get; set; }
        //public string correo  { get; set; }
        public string code  { get; set; }
      
    }
}
