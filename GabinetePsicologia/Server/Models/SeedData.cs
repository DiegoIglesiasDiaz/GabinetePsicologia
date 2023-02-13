using Microsoft.AspNetCore.Identity;

namespace GabinetePsicologia.Server.Models
{
    public static class SeedData
    {

        public static void Seed(UserManager<ApplicationUser> applicationUser,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(applicationUser);
        }
        public static void SeedUsers(UserManager<ApplicationUser> applicationUser)
        {
            if(applicationUser.FindByNameAsync("admin").Result == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@admin"
                };
                var result = applicationUser.CreateAsync(user,"Abrete01!").Result;
                if (result.Succeeded)
                {
                    applicationUser.AddToRoleAsync(user, "Administrador").Wait();
                }
            }
        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrador").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrador"

                };
               var result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Psicologo").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Psicologo"

                };
                var result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Paciente").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Paciente"

                };
                var result = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
