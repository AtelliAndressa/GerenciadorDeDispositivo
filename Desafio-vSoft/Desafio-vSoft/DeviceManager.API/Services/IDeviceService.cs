using DeviceManager.API.Models;

namespace DeviceManager.API.Services
{
    public interface IDeviceService
    {
        Task<List<Dispositivo>> GetAllAsync();

        Task<Dispositivo> GetByIdAsync(string id);

        Task<bool> CreateAsync(Dispositivo dispositivo);

        Task<bool> UpdateAsync(Dispositivo dispositivo);

        Task DeleteAsync(string id);

        Task<List<string>> CreateManyAsync(List<Dispositivo> dispositivos);
    }
}
