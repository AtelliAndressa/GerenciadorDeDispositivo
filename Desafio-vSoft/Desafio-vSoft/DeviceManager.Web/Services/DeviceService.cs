using DeviceManager.Web.Models;
using System.Net.Http.Json;

namespace DeviceManager.Web.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:44317/api/dispositivos";

        public DeviceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Dispositivo>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Dispositivo>>(BaseUrl);
        }

        public async Task<Dispositivo> GetByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<Dispositivo>($"{BaseUrl}/{id}");
        }

        public async Task<bool> CreateAsync(Dispositivo dispositivo)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dispositivo);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Dispositivo dispositivo)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{dispositivo.Id}", dispositivo);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
