using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NESCore;

namespace RobotNES;

public class ButtonMappingRow : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public IController.Keys Butt;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }
}