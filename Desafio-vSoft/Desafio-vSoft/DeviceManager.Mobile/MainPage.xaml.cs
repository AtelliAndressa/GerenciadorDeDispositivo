using DeviceManager.Mobile.Repositories;
using DeviceManager.Mobile.Services;
using DeviceManager.Mobile.ViewModels;

namespace DeviceManager.Mobile;

public partial class MainPage : ContentPage
{
    public MainPage(DispositivoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

