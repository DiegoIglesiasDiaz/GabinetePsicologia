using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class InformeDto : ICloneable
    {

        public InformeDto() 
        {
            Severidad = 1;
            lsInformeTrastornos = new List<InformeTrastorno>();
        }
        public Guid Id { get; set; }
        public List<InformeTrastorno> lsInformeTrastornos { get; set; }
 
        public string NombreTrastornos {
            get
            {
                string nombre = "";
                int count = 0;
                foreach (var item in lsInformeTrastornos)
                {
                    if(count>0) { nombre += ","; }
                    nombre += item.TrastornoName + " ("+item.TrastornoTipo+") ";
                    count ++;
                }
                return nombre;
            } 
        }
        public Guid PacienteId { get; set; }
        public string? PacienteFullName { get; set; }
      
        public Guid PsicologoId { get; set; }
        public string? PsicologoFullName { get; set; }
        public string? EvaluacionPsicologica { get; set; }
        public string? PlandDeTratamiento { get; set; }
        public string? Resultados { get; set; }
        public string? AntecendentesPersonales { get; set; }
        public string? Enlaces { get; set; }
        public string? EnlacesPrivate { get; set; }
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

        public object Clone()
        {
            return new InformeDto()
            {
                AntecendentesPersonales = AntecendentesPersonales,
                Enlaces = Enlaces,
                EnlacesPrivate = EnlacesPrivate,
                EvaluacionPsicologica = EvaluacionPsicologica,
                Id = Id,
                lsInformeTrastornos = lsInformeTrastornos,
                PacienteFullName = PacienteFullName,
                PacienteId = PacienteId,
                PlandDeTratamiento = PlandDeTratamiento,
                PsicologoFullName = PsicologoFullName,
                PsicologoId = PsicologoId,
                Resultados = Resultados,
                Severidad = Severidad,
                UltimaFecha = UltimaFecha

            };
        }
    }
}
