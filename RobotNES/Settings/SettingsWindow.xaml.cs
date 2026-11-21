using System.Windows;
using System.Windows.Input;

namespace RobotNES.Settings;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // Escape aborts an in-progress rebind (and does not close the dialog while doing so).
        if (e.Key != Key.Escape)
            return;

        if (DataContext is SettingsViewModel { IsAwaitingRebind: true } vm)
        {
            vm.CancelRebind();
            e.Handled = true;
        }
    }
}
