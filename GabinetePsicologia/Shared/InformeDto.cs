using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class InformeDto
    {
        public Guid Id { get; set; }
      
        public Guid TrastornoId { get; set; }
        public string TrastornoName { get; set; }
        public string TrastornoTipo { get; set; }

        public Guid PacienteId { get; set; }
        public string PacienteFullName { get; set; }
      
        public Guid PsicologoId { get; set; }
        public string PsicologoFullName { get; set; }
        public DateTime UltimaFecha { get; set; }
    }
}
