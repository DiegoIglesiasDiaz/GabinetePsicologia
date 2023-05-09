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
        public async void CrearOActalizarInforme(InformeDto Informe, bool isNew)
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
                Severidad = Informe.Severidad,
                TrastornoId = Informe.TrastornoId,
                UltimaFecha = Informe.UltimaFecha
                
                
            };
            if (isNew)
            {
                await _httpClient.PostAsJsonAsync("/Informe", inf);
            }
            else
            {
                await _httpClient.PostAsJsonAsync("/Informe/Actualizar", inf);
            }


        }
        public async Task<List<string[]>> ListFiles(string InformeId)
        {
            var inf = await _httpClient.GetFromJsonAsync<List<string[]>>($"/Informe/Files/{InformeId}");
            return inf;

        }
        public async void BorrarInforme(IList<InformeDto> inf)
        {
            await _httpClient.PostAsJsonAsync("/Informe/Borrar", inf);

        }
        public async void Descargar(string InformeId, string fileName)
        {

            var fileInfo = new string[] { InformeId, fileName };
            await _httpClient.PostAsJsonAsync($"/Informe/Files/Download",fileInfo);

        }
        public async void BorrarArchivo(string InformeId, string fileName)
        {

            var fileInfo = new string[] { InformeId, fileName };
            await _httpClient.PostAsJsonAsync($"/Informe/Files/Borrar", fileInfo);

        }
    }
}
