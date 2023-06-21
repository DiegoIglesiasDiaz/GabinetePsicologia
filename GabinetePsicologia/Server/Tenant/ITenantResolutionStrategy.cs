using Microsoft.EntityFrameworkCore;
using GabinetePsicologia.Shared;
using System.Net.Http;

namespace GabinetePsicologia.Server.Models
{
    public interface ITenantResolutionStrategy
    {
        Task<string> GetTenantIdentifierAsync();
    }
}