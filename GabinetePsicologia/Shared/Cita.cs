using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class Cita
    {
        public Guid Id { get; set; }
        public DateTime FecInicio { get; set; }
        public DateTime FecFin { get; set; }
        public string Nombre { get; set; }
        public Guid PacienteId { get; set; }
        public Guid PsicologoId { get; set; }
    }
}
