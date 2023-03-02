using GabinetePsicologia.Client.Pages.Psicologo;
using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Net;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected HttpClient _HttpClient { get; set; }

      
        protected ErrorBoundary? ErrorBoundary;

        protected override void OnParametersSet()
        {
            ErrorBoundary?.Recover();
        }
        protected void OnError(Exception e)
        {
            if (e is HttpRequestException httpException)
            {
                switch (httpException.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        NavigationManager.NavigateTo("/Logout", true);
                        break;
                    case HttpStatusCode.Forbidden:
                        NotificationService.Notify(NotificationSeverity.Error, "Acceso denegado",
                            "No tiene permisos para realizar esta acción");
                        break;
                    default:
                        NotificationService.Notify(NotificationSeverity.Error, "Error",
                            "Error de conexión, inténtelo de nuevo más tarde");
                        break;
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Ha ocurrido un error inesperado.");
            }
        }
      
    }
}
