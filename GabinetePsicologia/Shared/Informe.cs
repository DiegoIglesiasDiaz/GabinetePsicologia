using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class Informe
    {
        public Guid Id { get; set; }
        [ForeignKey("Trastorno")]
        public Guid TrastornoId { get; set; }
        public string TrastornoName { get; set; }
        [ForeignKey("Paciente")]
        public Guid PacienteId { get; set; }
        public string PacienteFullName { get; set; }
        [ForeignKey("Psicologo")]
        public Guid PsicologoId { get; set; }
        public string PsicologoFullName { get; set; }
        public DateTime UltimaFecha { get; set; }
    }
}
