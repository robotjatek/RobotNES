using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace RobotNES;

public class GamepadListener : IDisposable
{
    public event Action<GamepadButtons>? ButtonDown;
    public event Action<GamepadButtons>? ButtonUp;

    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(16)
    };

    private GamepadButtons _previousButtons;

    public GamepadListener()
    {
        _timer.Tick += (_, _) => PollController();
        _timer.Start();
    }

    public void Start()
    {
        _previousButtons = default;
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public void Dispose()
    {
        _timer.Stop();
        ButtonDown = null;
        ButtonUp = null;
    }

    private void PollController()
    {
        for (uint i = 0; i < 4; i++)
        {
            if (XInput.XInputGetState(i, out var state) != 0)
                continue;

            var current = (GamepadButtons)state.Gamepad.wButtons;
            var pressed = current & ~_previousButtons;
            var released = _previousButtons & ~current;

            foreach (var button in Enum.GetValues<GamepadButtons>())
            {
                if (button == 0)
                    continue;

                if ((pressed & button) != 0)
                {
                    ButtonDown?.Invoke(button);
                }

                if ((released & button) != 0)
                {
                    ButtonUp?.Invoke(button);
                }
            }

            _previousButtons = current;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_GAMEPAD
    {
        public ushort wButtons;
        public byte bLeftTrigger;
        public byte bRightTrigger;
        public short sThumbLX;
        public short sThumbLY;
        public short sThumbRX;
        public short sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct XINPUT_STATE
    {
        public uint dwPacketNumber;
        public XINPUT_GAMEPAD Gamepad;
    }

    private static class XInput
    {
        [DllImport("xinput1_4.dll")]
        public static extern uint XInputGetState(uint dwUserIndex, out XINPUT_STATE pState);
    }

    [Flags]
    public enum GamepadButtons : ushort
    {
        DPadUp    = 0x0001,
        DPadDown  = 0x0002,
        DPadLeft  = 0x0004,
        DPadRight = 0x0008,
        Start     = 0x0010,
        Back      = 0x0020,
        A         = 0x1000,
        B         = 0x2000,
        X         = 0x4000,
        Y         = 0x8000,
        LB        = 0x0100,
        RB        = 0x0200
    }
}
