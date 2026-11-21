using System.Collections.Generic;
using NESCore;

namespace RobotNES.Settings;

public class ControllerMappings
{
    public static ControllerMappings Instance { get; } = new();
    
    public Dictionary<GamepadListener.GamepadButtons, IController.Keys> ControllerMap { get; } = new()
    {
        { GamepadListener.GamepadButtons.A, IController.Keys.A },
        { GamepadListener.GamepadButtons.X, IController.Keys.B },
        { GamepadListener.GamepadButtons.Start, IController.Keys.START },
        { GamepadListener.GamepadButtons.Back, IController.Keys.SELECT },
        { GamepadListener.GamepadButtons.DPadDown,  IController.Keys.DOWN },
        { GamepadListener.GamepadButtons.DPadLeft,  IController.Keys.LEFT },
        { GamepadListener.GamepadButtons.DPadRight, IController.Keys.RIGHT },
        { GamepadListener.GamepadButtons.DPadUp, IController.Keys.UP }
    };
    
    // TODO: persist
}