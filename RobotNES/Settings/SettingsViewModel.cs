using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NESCore;

namespace RobotNES.Settings;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly GamepadListener _gamepadListener = new();

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private IController.Keys? _awaitingKey;

    public ICommand RebindCommand { get; }

    public ControllerMappings Mappings => ControllerMappings.Instance;
    
    public bool IsAwaitingRebind => _awaitingKey != null;
    
    public string RebindHint => IsAwaitingRebind ? "Press a button on the controller (Esc to cancel)" : "";

    public SettingsViewModel()
    {
        RebindCommand = new RelayCommand<string>(StartRebind);

        _gamepadListener.Start();
        _gamepadListener.ButtonDown += OnButtonDown;
    }

    private void StartRebind(string? nesButtonName)
    {
        if (!Enum.TryParse<IController.Keys>(nesButtonName, out var nesKey))
            return;

        if (_awaitingKey == nesKey)
            return;

        _awaitingKey = nesKey;
        RaiseAwaitingChanged();
    }
    
    public void CancelRebind()
    {
        if (_awaitingKey == null)
            return;

        _awaitingKey = null;
        RaiseAwaitingChanged();
    }

    private void OnButtonDown(GamepadListener.GamepadButtons gamepadButton)
    {
        if (_awaitingKey is not { } nesKey)
            return;

        var map = Mappings.ControllerMap;

        // The target NES key may currently be bound to a different controller button.
        // Find that stale owner first, then remove it after the loop so we never mutate
        // the dictionary while enumerating it.
        GamepadListener.GamepadButtons? staleOwner = null;
        foreach (var entry in map)
        {
            if (!Equals(entry.Value, nesKey) || Equals(entry.Key, gamepadButton)) continue;
            staleOwner = entry.Key;
            break;
        }

        if (staleOwner is { } stale)
            map.Remove(stale);

        map[gamepadButton] = nesKey;
        _awaitingKey = null;

        RaiseAwaitingChanged();
        OnPropertyChanged(nameof(Mappings));
    }

    private void RaiseAwaitingChanged()
    {
        OnPropertyChanged(nameof(IsAwaitingRebind));
        OnPropertyChanged(nameof(RebindHint));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
