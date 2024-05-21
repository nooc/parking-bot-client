using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParkingBot.ViewModels;

public abstract class BaseVm : INotifyPropertyChanged
{
    private bool isBusy = false;

    public Command LoadModelCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected BaseVm()
    {
        LoadModelCommand = new Command(ExecuteLoadModelCommand);
    }

    protected abstract void ExecuteLoadModelCommand();

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
