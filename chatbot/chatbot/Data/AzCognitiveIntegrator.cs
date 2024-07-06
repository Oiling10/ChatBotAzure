using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using chatbot.Models;

namespace chatbot.Services
{
    public interface IAzCognitiveIntegrator
    {
        Task<string> ObtenerRespuestaAsync(string mensaje);
    }

    public class AzCognitiveIntegrator : IAzCognitiveIntegrator
    {
        private readonly HttpClient _httpClient;
        private readonly AzureSettings _settings;
        private readonly ILogger<AzCognitiveIntegrator> _logger;

        public AzCognitiveIntegrator(HttpClient httpClient, IOptions<AzureSettings> settings, ILogger<AzCognitiveIntegrator> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> ObtenerRespuestaAsync(string mensaje)
        {
            try
            {
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString["query"] = mensaje;
                queryString["verbose"] = "true";
                queryString["show-all-intents"] = "false";
                queryString["log"] = "false";
                queryString["timezoneOffset"] = "0";

                var endpointUri = $"{_settings.CognitiveServicesEndpoint}language/analyze-text?{queryString}";

                _logger.LogInformation($"Endpoint URI: {endpointUri}");

                var request = new HttpRequestMessage(HttpMethod.Get, endpointUri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _settings.CognitiveServicesApiKey);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error en la llamada a Azure Cognitive Services API: {response.StatusCode} - {response.ReasonPhrase}");
                    return "Error en la llamada a Azure Cognitive Services API";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var luisResponse = JsonConvert.DeserializeObject<LUISResponse>(responseContent);
                return responseContent;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error en la solicitud HTTP al API de Azure.");
                return $"Error en la solicitud HTTP: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al obtener la respuesta del API.");
                return $"Ocurrió un error: {ex.Message}";
            }
        }
    }

    public class LUISResponse
    {
        public Prediction prediction { get; set; }
    }

    public class Prediction
    {
        public string topIntent { get; set; }
    }

    public class AzureSettings
    {
        public string CognitiveServicesApiKey { get; set; }
        public string CognitiveServicesEndpoint { get; set; }
        public string ProjectName { get; set; }
    }
}
