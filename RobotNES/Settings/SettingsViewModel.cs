using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RobotNES.Settings;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly GamepadListener _gamepadListener = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raw last button pressed, for logic that needs the enum value.</summary>
    public GamepadListener.GamepadButtons PressedKey
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            OnPropertyChanged();
        }
    }

    public SettingsViewModel()
    {
        _gamepadListener.Start();
        _gamepadListener.ButtonDown += button => { PressedKey = button; };
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}