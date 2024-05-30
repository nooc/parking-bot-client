using Microsoft.Extensions.Logging;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParkingBot.ViewModels;

public abstract class BaseVm : INotifyPropertyChanged
{
    protected readonly ILogger _logger;
    private bool isBusy = false;

    public Command LoadModelCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected BaseVm(ILogger logger)
    {
        _logger = logger;
        LoadModelCommand = new Command(_ExecuteLoadModelCommand);
    }

    protected abstract void ExecuteLoadModelCommand();

    private void _ExecuteLoadModelCommand()
    {
        isBusy = true;
        try
        {
            ExecuteLoadModelCommand();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, message: null);
        }
        finally
        {
            isBusy = false;
        }
    }

    public bool IsBusy
    {
        get { return isBusy; }
        set { SetProperty(ref isBusy, value); }
    }

    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
