using DeviceManager.Mobile.Models;

namespace DeviceManager.Mobile.Interfaces
{
    public interface IDeviceRepository
    {
        Task<List<Dispositivo>> GetAllAsync();

        Task AddAsync(Dispositivo dispositivo);

        Task UpdateAsync(Dispositivo dispositivo);

        Task DeleteAsync(Dispositivo dispositivo);

        Task<List<Dispositivo>> GetUnsynchronizedAsync();

        Task<bool> CodigoReferenciaExisteAsync(string codigoReferencia);
    }
}
