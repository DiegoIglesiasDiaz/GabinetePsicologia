using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Mvc;


namespace GabinetePsicologia.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class TenantController : Controller
    {
        private readonly TenantAccessService<Tenant> _tenantService;

        private readonly ILogger<TenantController> _logger;

        public TenantController(ILogger<TenantController> logger, TenantAccessService<Tenant> tenantService)
        {
            _logger = logger;
            _tenantService = tenantService;
        }


        [HttpGet()]
        public Tenant GetTenant()
        {
            return HttpContext.GetTenant();
        }
    }
}