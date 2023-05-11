using Duende.IdentityServer.EntityFramework.Options;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GabinetePsicologia.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
   
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions
         ) : base(options, operationalStoreOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().Navigation(e => e.LsAdmin).AutoInclude();
            builder.Entity<ApplicationUser>().Navigation(e => e.LsPsicologo).AutoInclude();
            builder.Entity<ApplicationUser>().Navigation(e => e.LsPaciente).AutoInclude();

            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Paciente", NormalizedName = "Paciente", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Psicologo", NormalizedName = "Psicologo", Id = Guid.NewGuid().ToString(), ConcurrencyStamp = Guid.NewGuid().ToString() });

        }
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Psicologo> Psicologos => Set<Psicologo>();
        public DbSet<Administrador> Administradores => Set<Administrador>();
        public DbSet<Trastorno> Trastornos => Set<Trastorno>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Informe> Informes => Set<Informe>();
        public DbSet<InformeTrastorno> InformeTrastorno => Set<InformeTrastorno>();

    }
}