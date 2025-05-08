using DeviceManager.Mobile.Interfaces;
using DeviceManager.Mobile.Models;
using DeviceManager.Mobile.ViewModels;
using Newtonsoft.Json;
using Realms;
using System.Net.Http.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace DeviceManager.Mobile.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly HttpClient _client;
        private const string BaseUrl = "https://localhost:44317/api/dispositivos";

        public DeviceService()
        {
            _client = new HttpClient();
        }

        public async Task<List<Dispositivo>> GetAllAsync()
        {
            var response = await _client.GetStringAsync(BaseUrl);
            return JsonConvert.DeserializeObject<List<Dispositivo>>(response);
        }

        public async Task<bool> CreateAsync(Dispositivo dispositivo)
        {
            var json = JsonConvert.SerializeObject(dispositivo);
            var response = await _client.PostAsync(BaseUrl, new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Dispositivo dispositivo)
        {
            var json = JsonConvert.SerializeObject(dispositivo);
            var response = await _client.PutAsync($"{BaseUrl}/{dispositivo.Id}", new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateManyAsync(List<Dispositivo> dispositivos)
        {
            var json = JsonConvert.SerializeObject(dispositivos);
            var response = await _client.PostAsync($"{BaseUrl}/lote", new StringContent(json, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
