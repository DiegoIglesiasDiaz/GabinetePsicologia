using Microsoft.EntityFrameworkCore;
using GabinetePsicologia.Shared;
using System.Net.Http;

namespace GabinetePsicologia.Server.Models
{
    public interface ITenantStore<T> where T : Tenant
    {
        Task<T> GetTenantAsync(string identifier);
    }
}