using DeviceManager.API.Models;
using MongoDB.Driver;

namespace DeviceManager.API.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IMongoCollection<Dispositivo> _dispositivos;

        public DeviceRepository(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _dispositivos = database.GetCollection<Dispositivo>(config["MongoDB:DispositivoCollection"]);
        }

        public async Task<List<Dispositivo>> GetAllAsync() =>
            await _dispositivos.Find(_ => true).ToListAsync();

        public async Task<Dispositivo> GetByIdAsync(string id) =>
            await _dispositivos.Find(d => d.Id == id).FirstOrDefaultAsync();

        public async Task<Dispositivo> GetByCodigoReferenciaAsync(string codigoReferencia) =>
            await _dispositivos.Find(d => d.CodigoReferencia == codigoReferencia).FirstOrDefaultAsync();

        public async Task CreateAsync(Dispositivo dispositivo) =>
            await _dispositivos.InsertOneAsync(dispositivo);

        public async Task UpdateAsync(Dispositivo dispositivo) =>
            await _dispositivos.ReplaceOneAsync(d => d.Id == dispositivo.Id, dispositivo);

        public async Task DeleteAsync(string id) =>
            await _dispositivos.DeleteOneAsync(d => d.Id == id);

        public async Task CreateManyAsync(List<Dispositivo> dispositivos)
        {
            await _dispositivos.InsertManyAsync(dispositivos);
        }
    }
}
