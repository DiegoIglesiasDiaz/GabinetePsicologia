using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class InformeDto
    {
        public InformeDto() { Severidad = 1; }
        public Guid Id { get; set; }
      
        public Guid TrastornoId { get; set; }
        public string TrastornoName { get; set; }
        public string TrastornoTipo { get; set; }
        public string NombreTipoTrastorno
        {
            get
            {
                return TrastornoTipo + "  " + TrastornoName;
            }
        }
        public Guid PacienteId { get; set; }
        public string PacienteFullName { get; set; }
      
        public Guid PsicologoId { get; set; }
        public string PsicologoFullName { get; set; }
        public string EvaluacionPsicologica { get; set; }
        public string PlandDeTratamiento { get; set; }
        public string Resultados { get; set; }
        public string AntecendentesPersonales { get; set; }
        public DateTime UltimaFecha { get; set; }
        public string UltimaFechaString {
            get
            {
                return UltimaFecha.ToString("dd/MM/yyyy");
            }
        }
        public string UltimaFechaString2
        {
            get
            {
                return UltimaFecha.ToString("dd/MM/yyyy HH:mm");
            }
        }
        public int Severidad { get; set; }
    }
}
