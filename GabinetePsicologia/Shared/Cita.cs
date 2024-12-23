﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("Paciente")]
        public Guid PacienteId { get; set; }
        [ForeignKey("Psicologo")]
        public Guid PsicologoId { get; set; }
        
        public string? PacienteFullName  { get; set; }
        public string? PsicologoFullName  { get; set; }
    }
}
