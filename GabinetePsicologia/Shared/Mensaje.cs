using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class Mensaje
    {
        public Mensaje() { 
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Asunto { get; set; }
        public string Correo { get; set; }
        public string MensajeCuerpo { get; set; }
        public bool Visto { get; set; }
    }
}
