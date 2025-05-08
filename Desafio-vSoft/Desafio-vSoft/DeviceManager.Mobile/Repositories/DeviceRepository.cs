using DeviceManager.Mobile.Interfaces;
using DeviceManager.Mobile.Models;
using Realms;

namespace DeviceManager.Mobile.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        //private Realm _realm => Realm.GetInstance();
        private Realm _realm => Realm.GetInstance(new RealmConfiguration { ShouldDeleteIfMigrationNeeded = true });


        public Task<List<Dispositivo>> GetAllAsync()
        {
            return Task.FromResult(_realm.All<Dispositivo>().Where(d => !d.IsDeleted).ToList());
        }

        public async Task AddAsync(Dispositivo dispositivo)
        {
            try
            {
                await _realm.WriteAsync(() =>
                {
                    _realm.Add(dispositivo);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAsync(Dispositivo dispositivo)
        {
            await _realm.WriteAsync(() =>
            {
                dispositivo.DataAtualizacao = System.DateTime.Now;
                dispositivo.IsSynchronized = false;
                _realm.Add(dispositivo, update: true);
            });
        }

        public async Task DeleteAsync(Dispositivo dispositivo)
        {
            await _realm.WriteAsync(() =>
            {
                dispositivo.IsDeleted = true;
                dispositivo.IsSynchronized = false;
                _realm.Add(dispositivo, update: true);
            });
        }

        public Task<List<Dispositivo>> GetUnsynchronizedAsync()
        {
            return Task.FromResult(_realm.All<Dispositivo>().Where(d => !d.IsSynchronized).ToList());
        }

        public Task<bool> CodigoReferenciaExisteAsync(string codigoReferencia)
        {
            return Task.FromResult(_realm.All<Dispositivo>().Any(d => d.CodigoReferencia == codigoReferencia && !d.IsDeleted));
        }
    }
}

