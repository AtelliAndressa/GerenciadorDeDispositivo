using DeviceManager.Web.Models;

namespace DeviceManager.Web.Services
{
    public interface IDeviceService
    {
        Task<List<Dispositivo>> GetAllAsync();
        Task<Dispositivo> GetByIdAsync(string id);
        Task<bool> CreateAsync(Dispositivo dispositivo);
        Task<bool> UpdateAsync(Dispositivo dispositivo);
        Task<bool> DeleteAsync(string id);
    }
} 