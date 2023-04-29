﻿
using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Principal;

namespace GabinetePsicologia.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; }
        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected HttpClient _HttpClient { get; set; }
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] protected UsuarioServices UsuarioServices { get; set; }
        string Name;
        public bool isAdmin = false;
        protected ErrorBoundary? ErrorBoundary;

        protected override void OnParametersSet()
        {
            ErrorBoundary?.Recover();
            
        }
        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user != null && (user.IsInRole("Administrador") || user.IsInRole("Psicologo") || user.IsInRole("Paciente")))
            {
                PersonaDto userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                Name = userDto.FullName;
                if (!user.IsInRole("Paciente"))
                {
                    isAdmin= true;
                }
                if (userDto.Contraseña == null)
                {
                    var result = await DialogService.OpenAsync<IndexModal>("Información Adicional", new Dictionary<string, object> { { "Persona", userDto } });
                    if (result)
                        NotificationService.Notify(NotificationSeverity.Success, "Ok", "Datos Guardado Correctamente");
                }
            }


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
