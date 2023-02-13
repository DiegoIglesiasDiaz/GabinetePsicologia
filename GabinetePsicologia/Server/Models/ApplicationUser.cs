using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Identity;

namespace GabinetePsicologia.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<Paciente> LsPaciente { get; set; } = new List<Paciente>();
        //public int Rol { get; set; }

    }
}