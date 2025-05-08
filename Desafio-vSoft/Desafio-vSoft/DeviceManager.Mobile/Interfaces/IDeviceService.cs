using DeviceManager.Mobile.Models;

namespace DeviceManager.Mobile.Interfaces
{
    public interface IDeviceService
    {
        Task<List<Dispositivo>> GetAllAsync();

        Task<bool> CreateAsync(Dispositivo dispositivo);

        Task<bool> UpdateAsync(Dispositivo dispositivo);

        Task<bool> DeleteAsync(string id);

        Task<bool> CreateManyAsync(List<Dispositivo> dispositivo);
    }
}
