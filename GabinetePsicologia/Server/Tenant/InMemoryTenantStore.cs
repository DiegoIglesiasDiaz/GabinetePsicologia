using Microsoft.EntityFrameworkCore;
using GabinetePsicologia.Shared;
using System.Net.Http;

namespace GabinetePsicologia.Server.Models
{
    /// <summary>
    /// In memory store for testing
    /// </summary>
    public class InMemoryTenantStore : ITenantStore<Tenant>
    {
        public Tenant[] tenant { get; } = new[]
                {
                new Tenant{  Id = Guid.NewGuid(), Identifier = "localhost", NombreApp = "GabinetePsicologia", Database="GabinetePsicologia" },
                new Tenant{  Id = Guid.NewGuid(), Identifier = "diegoiglesiasdiaz.com", NombreApp = "GabinetePsicologia", Database="GabinetePsicologia" },
                new Tenant{ Id = Guid.NewGuid(), Identifier = "centrodetecnicasnaturalesneo.com", NombreApp = "Centro de Técnicas Naturales Neo", Database="ClinicaNeo"  }
                };
/// <summary>
/// Get a tenant for a given identifier
/// </summary>
/// <param name="identifier"></param>
/// <returns></returns>
public async Task<Tenant> GetTenantAsync(string identifier)
        {
             return await Task.FromResult(tenant.SingleOrDefault(t => identifier.ToLower().Contains(t.Identifier.ToLower())));
        }

    }
}