﻿
using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
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
        [Inject] protected ChatServices ChatServices { get; set; }
		[Inject] private IJSRuntime jSRuntime { get; set; }
		string Name;
        ClaimsPrincipal? user;
        public bool isAdmin = false;
        protected ErrorBoundary? ErrorBoundary;
		private HubConnection? hubConnectionlayout;

		protected override void OnParametersSet()
        {
            ErrorBoundary?.Recover();
            
        }
        protected override async Task OnInitializedAsync()
        {
            user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user != null && (user.IsInRole("Administrador") || user.IsInRole("Psicologo") || user.IsInRole("Paciente")))
            {
                PersonaDto userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                Name = userDto.Nombre;
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
                var resultBool = await ChatServices.hasNonViewMessage(userDto.Id.ToString());
                if (resultBool)
                {
					await jSRuntime.InvokeVoidAsync("MessageOnShow");
				}
                else{
					await jSRuntime.InvokeVoidAsync("MessageOnHide");
				}

				try
				{
					await Connect();

				}
				catch(Exception ex)
                {

                }

              
            }
        }
		private async Task Connect()
		{

			hubConnectionlayout = new HubConnectionBuilder()
							.WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
							.Build();

			hubConnectionlayout.On<string, string>("NotificationMessage", HandleReceivedMessage);
			await hubConnectionlayout.StartAsync();
		}

		public async ValueTask DisposeAsync()
		{
			if (hubConnectionlayout != null)
			{
				await hubConnectionlayout.DisposeAsync();
			}
		}

		private async void HandleReceivedMessage(string usr, string message)
		{
			var split = usr.Split(";");
			//var FromUser = split[0];
			var ToUser = split[1];
			var FromName = split[2];

			var LogUser = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
			if (ToUser == LogUser.Id.ToString())
			{
                if (!NavigationManager.Uri.EndsWith("/Chat"))
                {
					NotificationService.Notify(NotificationSeverity.Info, "", $"Nuevo Mensaje de {FromName}.");
				}
               
				await jSRuntime.InvokeVoidAsync("MessageOnShow");
				await jSRuntime.InvokeVoidAsync("sonidoMssg");

			}

			StateHasChanged();
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
