﻿using System;
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
     
        [ForeignKey("Paciente")]
        public Guid PacienteId { get; set; }

        [ForeignKey("Psicologo")]
        public Guid PsicologoId { get; set; }
        public string? EvaluacionPsicologica { get; set; }
        public string? PlandDeTratamiento { get; set; }
        public string? Resultados { get; set; }
        public string? AntecendentesPersonales { get; set; }
        public string? Enlaces { get; set; }
        public string? EnlacesPrivate { get; set; }
        public DateTime UltimaFecha { get; set; }
    }
}
