using DeviceManager.API.Models;
using DeviceManager.API.Repositories;
using DeviceManager.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DeviceManager.API.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _repository;
        private readonly string _logPath = "Logs/LogSincronizacao.txt";

        public DeviceService(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Dispositivo>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<Dispositivo> GetByIdAsync(string id) => await _repository.GetByIdAsync(id);

        public async Task<bool> CreateAsync(Dispositivo dispositivo)
        {
            if (await _repository.GetByCodigoReferenciaAsync(dispositivo.CodigoReferencia) != null)
                return false;

            dispositivo.DataCriacao = DateTime.UtcNow;
            await _repository.CreateAsync(dispositivo);
            Log("CREATE", dispositivo);
            return true;
        }

        public async Task<bool> UpdateAsync(Dispositivo dispositivo)
        {
            var existente = await _repository.GetByIdAsync(dispositivo.Id);
            if (existente == null) return false;

            dispositivo.DataCriacao = existente.DataCriacao;
            dispositivo.DataAtualizacao = DateTime.UtcNow;
            await _repository.UpdateAsync(dispositivo);
            Log("UPDATE", dispositivo);
            return true;
        }

        public async Task DeleteAsync(string id)
        {
            var dispositivo = await _repository.GetByIdAsync(id);
            if (dispositivo != null)
            {
                await _repository.DeleteAsync(id);
                Log("DELETE", dispositivo);
            }
        }

        public async Task<List<string>> CreateManyAsync(List<Dispositivo> dispositivos)
        {
            var codigosRejeitados = new List<string>();
            var codigosUnicos = new HashSet<string>();
            var dispositivosParaInserir = new List<Dispositivo>();

            // Verifica duplicidade na própria lista recebida
            foreach (var d in dispositivos)
            {
                if (!codigosUnicos.Add(d.CodigoReferencia))
                {
                    codigosRejeitados.Add(d.CodigoReferencia + " (duplicado na lista)");
                    continue;
                }

                // Verifica duplicidade no banco
                if (await _repository.GetByCodigoReferenciaAsync(d.CodigoReferencia) != null)
                {
                    codigosRejeitados.Add(d.CodigoReferencia + " (já existe no banco)");
                    continue;
                }

                d.DataCriacao = DateTime.UtcNow;
                dispositivosParaInserir.Add(d);
            }

            if (dispositivosParaInserir.Any())
            {
                await _repository.CreateManyAsync(dispositivosParaInserir);
                foreach (var d in dispositivosParaInserir)
                    Log("CREATE", d);
            }

            return codigosRejeitados;
        }

        private void Log(string acao, Dispositivo d)
        {
            Directory.CreateDirectory("Logs");
            File.AppendAllText(_logPath, $"{DateTime.Now} - {acao} - ID: {d.Id}, Ref: {d.CodigoReferencia}\n");
        }
    }
}
