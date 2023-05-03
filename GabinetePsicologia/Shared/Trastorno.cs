using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
    public class Trastorno
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        [AllowNull]

        public string? Sintomas { get; set; }
        public string NombreTipo
        {
            get
            {
                return Tipo + "  " + Nombre;
            }
        }

    }
}
