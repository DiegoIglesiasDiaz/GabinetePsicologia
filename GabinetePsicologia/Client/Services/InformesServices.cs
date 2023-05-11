using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Services
{
    public class InformesServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;
        public InformesServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;

        }
        public async Task<List<InformeDto>> GetInformes()
        {
            var inf = await _httpClient.GetFromJsonAsync<List<InformeDto>>("/Informe");
            return inf;

        }
        public async Task<List<InformeDto>> GetInformesById(Guid id)
        {

            var inf = await _httpClient.GetFromJsonAsync<List<InformeDto>>($"/Informe/{id}");
            return inf;

        }
        public async void CrearOActalizarInforme(InformeDto Informe, bool isNew, bool ForceReload)
        {
            Informe inf = new Informe()
            {
                AntecendentesPersonales = Informe.AntecendentesPersonales,
                EvaluacionPsicologica = Informe.EvaluacionPsicologica,
                Id = Informe.Id,
                PacienteId = Informe.PacienteId,
                PlandDeTratamiento = Informe.PlandDeTratamiento,
                PsicologoId = Informe.PsicologoId,
                Resultados = Informe.Resultados,
                UltimaFecha = Informe.UltimaFecha,
                Enlaces = Informe.Enlaces,
                EnlacesPrivate = Informe.EnlacesPrivate


            };


            if (isNew)
            {
                foreach (var item in Informe.lsInformeTrastornos)
                {
                    item.InformeId = Informe.Id;
                }
                await _httpClient.PostAsJsonAsync("/Informe", inf);
            }
            else
            {
                await _httpClient.PostAsJsonAsync("/Informe/Actualizar", inf);
            }

            await _httpClient.PostAsJsonAsync("/Informe/InformeTrastorno/Delete", inf);
            if (Informe.lsInformeTrastornos.Count > 0)
            {
                await _httpClient.PostAsJsonAsync("/Informe/InformeTrastorno/Insertar", Informe.lsInformeTrastornos);
            }
            if (ForceReload)
            {
                _navigationManager.NavigateTo("/Informes", true);
            }

        }
        public async Task<List<string[]>> ListFiles(string InformeId)
        {
            var inf = await _httpClient.GetFromJsonAsync<List<string[]>>($"/Informe/Files/{InformeId}");
            return inf;

        }
        public async Task<List<string[]>> ListFilesPaciente(string InformeId)
        {
            var inf = await _httpClient.GetFromJsonAsync<List<string[]>>($"/Informe/FilesPaciente/{InformeId}");
            return inf;

        }
        public async void BorrarInforme(IList<InformeDto> inf)
        {
            await _httpClient.PostAsJsonAsync("/Informe/Borrar", inf);

        }
        public async void Descargar(string InformeId, string fileName)
        {

            var fileInfo = new string[] { InformeId, fileName };
            await _httpClient.PostAsJsonAsync($"/Informe/Files/Download", fileInfo);

        }
        public async void BorrarArchivo(string InformeId, string fileName)
        {

            var fileInfo = new string[] { InformeId, fileName };
            await _httpClient.PostAsJsonAsync($"/Informe/Files/Borrar", fileInfo);

        }
        public async void setEnlaces(string id, string enlace)
        {

            var enlaceInfo = new string[] { id, enlace };
            await _httpClient.PostAsJsonAsync($"/Informe/Enlaces", enlaceInfo);

        }
        public async void setEnlacesPrivado(string id, string enlace)
        {
            var enlaceInfo = new string[] { id, enlace };
            await _httpClient.PostAsJsonAsync($"/Informe/EnlacesPrivate", enlaceInfo);

        }
        public async void setSeveridad(Guid id, int Severidad)
        {
            var Data = new string[] { id.ToString(), Severidad.ToString() };
            await _httpClient.PostAsJsonAsync($"/Informe/Severidad", Data);

        }
    }
}
