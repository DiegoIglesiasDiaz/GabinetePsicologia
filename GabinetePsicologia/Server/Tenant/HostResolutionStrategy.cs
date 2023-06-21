using Microsoft.EntityFrameworkCore;
using GabinetePsicologia.Shared;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace GabinetePsicologia.Server.Models
{
    /// <summary>
    /// Resolve the host to a tenant identifier
    /// </summary>
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HostResolutionStrategy(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get the tenant identifier
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> GetTenantIdentifierAsync()
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext.Request.Host.Host);
        }
        public string GetConnectionString()
        {
            if (_httpContextAccessor.HttpContext == null) return "ERROR: Ninguna Cadena de Conexion a BBDD";
            string host = _httpContextAccessor.HttpContext.Request.Host.Host;

            // Busca la información de conexión del inquilino basándote en el host
            InMemoryTenantStore memoryTenantStore = new InMemoryTenantStore();
            Tenant tenant = memoryTenantStore.tenant.FirstOrDefault(t => t.Identifier == host);
            
            if (tenant == null)
            {
                string a = _httpContextAccessor.HttpContext.Request.Host.Host;
                // Maneja el caso en el que el host no se corresponda con ningún inquilino
                //throw new Exception("No se encontró ningún inquilino para el host proporcionado.");
                throw new Exception(a);
            }

          
            // Construye la cadena de conexión
            return $"Server=217.160.115.84;Database={tenant.Database};User ID=sa;Password=Abrete01;Encrypt=false;";

        }
    }
}