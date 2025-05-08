using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DeviceManager.Mobile.Interfaces;
using DeviceManager.Mobile.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Mobile.ViewModels
{
    public partial class DispositivoViewModel : ObservableObject
    {
        private readonly IDeviceRepository _repository;

        private readonly IDeviceService _apiService;

        public ObservableCollection<Dispositivo> Dispositivos { get; } = new();

        public ObservableCollection<string> Logs { get; } = new();

        [ObservableProperty] string descricao;

        [ObservableProperty] string codigoReferencia;

        [ObservableProperty] string editDescricao;

        [ObservableProperty] string editCodigoReferencia;

        [ObservableProperty] bool isEditing;

        public ICommand AddCommand { get; }

        public ICommand SyncCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand UpdateCommand { get; }

        public ICommand ShowEditPanelCommand { get; }

        public ICommand CancelUpdateCommand { get; }

        private Dispositivo dispositivoEditando;

        public DispositivoViewModel() { }

        public DispositivoViewModel(IDeviceRepository repository, IDeviceService apiService)
        {
            _repository = repository;
            _apiService = apiService;

            AddCommand = new AsyncRelayCommand(AddDispositivoAsync);

            SyncCommand = new AsyncRelayCommand(SyncAsync);

            UpdateCommand = new AsyncRelayCommand(UpdateAsync);

            DeleteCommand = new AsyncRelayCommand<Dispositivo>(DeleteAsync);

            ShowEditPanelCommand = new RelayCommand<Dispositivo>(ShowEditPanel);

            CancelUpdateCommand = new RelayCommand(CancelUpdate);


            LoadData();
        }

        private async void LoadData()
        {
            Dispositivos.Clear();

            var lista = await _repository.GetAllAsync();

            if (lista != null)
            {
                foreach (var item in lista)
                    Dispositivos.Add(item);
            }
        }

        private async Task AddDispositivoAsync()
        {
            if (await _repository.CodigoReferenciaExisteAsync(codigoReferencia))
            {
                Logs.Add($"Código {codigoReferencia} já existe.");
                return;
            }

            var novo = new Dispositivo
            {
                Descricao = descricao,
                CodigoReferencia = codigoReferencia,
                IsSynchronized = false
            };

            await _repository.AddAsync(novo);

            Dispositivos.Add(novo);

            Logs.Add($"[{System.DateTime.Now:T}] Adicionado: {descricao}");
        }

        private void ShowEditPanel(Dispositivo dispositivo)
        {
            dispositivoEditando = dispositivo;

            EditDescricao = dispositivo.Descricao;
            EditCodigoReferencia = dispositivo.CodigoReferencia;
            isEditing = true;
        }

        private void CancelUpdate()
        {
            EditDescricao = string.Empty;
            EditCodigoReferencia = string.Empty;
            isEditing = false;
        }

        private async Task UpdateAsync()
        {
            if (dispositivoEditando == null)
                return;

            dispositivoEditando.Descricao = EditDescricao;
            dispositivoEditando.CodigoReferencia = EditCodigoReferencia;
            dispositivoEditando.DataAtualizacao = DateTime.Now;
            dispositivoEditando.IsSynchronized = false;

            await _repository.UpdateAsync(dispositivoEditando);

            Logs.Add($"[{DateTime.Now:T}] Editado: {dispositivoEditando.Descricao}");

            dispositivoEditando = null;
            isEditing = false;
            EditDescricao = string.Empty;
            EditCodigoReferencia = string.Empty;

            LoadData();
        }


        private async Task DeleteAsync(Dispositivo dispositivo)
        {
            await _repository.DeleteAsync(dispositivo);

            Dispositivos.Remove(dispositivo);

            Logs.Add($"[{System.DateTime.Now:T}] Marcado como excluído: {dispositivo.Descricao}");
        }

        private async Task SyncAsync()
        {
            Logs.Add($"[{System.DateTime.Now:T}] Iniciando sincronização...");

            var remotos = await _apiService.GetAllAsync();
            var pendentes = await _repository.GetUnsynchronizedAsync();

            // Separa os dispositivos pendentes em listas por operação
            var paraCriar = new List<Dispositivo>();
            var paraAtualizar = new List<Dispositivo>();
            var paraExcluir = new List<Dispositivo>();

            foreach (var d in pendentes)
            {
                var codigoConflito = remotos.Any(r => r.CodigoReferencia == d.CodigoReferencia && r.Id != d.Id);
                if (codigoConflito)
                {
                    Logs.Add($"Conflito: Código '{d.CodigoReferencia}' já existe na API. Dispositivo {d.Id} ignorado.");
                    continue;
                }

                if (d.IsDeleted)
                {
                    paraExcluir.Add(d);
                }
                else if (remotos.Any(r => r.Id == d.Id))
                {
                    paraAtualizar.Add(d);
                }
                else
                {
                    paraCriar.Add(d);
                }
            }

            // Envia os dispositivos para criar em lote
            if (paraCriar.Any())
            {
                var resultado = await _apiService.CreateManyAsync(paraCriar);
                if (resultado)
                {
                    foreach (var d in paraCriar)
                    {
                        d.IsSynchronized = true;
                        await _repository.UpdateAsync(d);
                        Logs.Add($"Criado remotamente: {d.Id}");
                    }
                }
                else
                {
                    Logs.Add("Erro ao criar dispositivos em lote.");
                }
            }

            // Atualiza e exclui individualmente
            foreach (var d in paraAtualizar)
            {
                var resultado = await _apiService.UpdateAsync(d);
                if (resultado)
                {
                    d.IsSynchronized = true;
                    await _repository.UpdateAsync(d);
                    Logs.Add($"Atualizado remotamente: {d.Id}");
                }
                else
                {
                    Logs.Add($"Erro ao atualizar: {d.Id}");
                }
            }

            foreach (var d in paraExcluir)
            {
                var resultado = await _apiService.DeleteAsync(d.Id);
                if (resultado)
                {
                    d.IsSynchronized = true;
                    await _repository.UpdateAsync(d);
                    Logs.Add($"Excluído remotamente: {d.Id}");
                }
                else
                {
                    Logs.Add($"Erro ao excluir: {d.Id}");
                }
            }

            // Importar dados da API para o app
            foreach (var remoto in remotos)
            {
                var local = Dispositivos.FirstOrDefault(d => d.Id == remoto.Id);
                if (local == null)
                {
                    await _repository.AddAsync(remoto);
                    Logs.Add($"Importado do servidor: {remoto.Descricao}");
                }
                else if (remoto.DataAtualizacao > local.DataAtualizacao)
                {
                    await _repository.UpdateAsync(remoto);
                    Logs.Add($"[{System.DateTime.Now:T}] Atualizado local com dados do servidor: {remoto.Descricao}");
                }
            }

            LoadData();
            Logs.Add("Sincronização finalizada.");
        }

        public event PropertyChangingEventHandler PropertyChanged;
    }
} 

