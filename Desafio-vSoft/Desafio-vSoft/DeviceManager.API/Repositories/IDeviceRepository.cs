using DeviceManager.API.Models;

namespace DeviceManager.API.Repositories
{
    public interface IDeviceRepository
    {
        Task<List<Dispositivo>> GetAllAsync();

        Task<Dispositivo> GetByIdAsync(string id);

        Task<Dispositivo> GetByCodigoReferenciaAsync(string codigoReferencia);

        Task CreateAsync(Dispositivo dispositivo);

        Task UpdateAsync(Dispositivo dispositivo);

        Task DeleteAsync(string id);

        Task CreateManyAsync(List<Dispositivo> dispositivos);
    }
}
