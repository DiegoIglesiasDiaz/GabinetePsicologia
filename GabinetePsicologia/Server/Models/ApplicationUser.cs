using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Identity;

namespace GabinetePsicologia.Server.Models
{
    public class ApplicationUser : IdentityUser
    {

        public List<Paciente> LsPaciente { get; set; } = new List<Paciente>();
        public List<Psicologo> LsPsicologo { get; set; } = new List<Psicologo>();
        public List<Administrador> LsAdmin { get; set; } = new List<Administrador>();
        //public int Rol { get; set; }

    }
}