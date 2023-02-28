using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
namespace GabinetePsicologia.Client.Shared
{
    public partial class LoginDisplay : LayoutComponentBase
    {
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        protected ErrorBoundary ErrorBoundary;

        protected override void OnParametersSet()
        {
            ErrorBoundary?.Recover();
        }

        protected void OnError(Exception e)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Error de conexión, inténtelo de nuevo más tarde.");
            NavigationManager.NavigateTo("/Login", true);
        }
        protected void Volver()
        {
            NavigationManager.NavigateTo("/", true);
        }
    }
}
