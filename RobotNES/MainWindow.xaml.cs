using NESCore;

using Serilog;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RobotNES
{
    public partial class MainWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        struct XINPUT_GAMEPAD
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
        struct XINPUT_STATE
        {
            public uint dwPacketNumber;
            public XINPUT_GAMEPAD Gamepad;
        }
        
        static class XInput
        {
            [DllImport("xinput1_4.dll")]
            public static extern uint XInputGetState(
                uint dwUserIndex,
                out XINPUT_STATE pState);
        }
        
        private readonly DispatcherTimer _timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        
        private readonly Stopwatch _stopwatch = new();
        private int _frameIndex;
        private readonly double[] _frameTimes = new double[60];

        private NesSystem? _nesSystem;
        private readonly ILogger _logger;
        private readonly WriteableBitmap _bitmap = new(256, 240, 96.0, 96.0, PixelFormats.Bgr24, null);
        private readonly Int32Rect _bitmapRect = new(0, 0, 256, 240);
        private readonly AudioPlayer _audioPlayer = new();
        
        private GamepadButtons _previousButtons;

        public MainWindow()
        {
            InitializeComponent();
            _logger = CreateLogger();
            _timer.Tick += (_,_) => PollController();
            _timer.Start();
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
                        ButtonDown(button);
                    }

                    if ((released & button) != 0)
                    {
                        ButtonUp(button);
                    }
                }
                
                _previousButtons = current;
            }
        }

        void ButtonDown(GamepadButtons button)
        {
            if (_controllerMap.TryGetValue(button, out var value))
            {
                _nesSystem?.PressControllerButton(value);
            }
        }

        void ButtonUp(GamepadButtons button)
        {
            if (_controllerMap.TryGetValue(button, out var value))
            {
                _nesSystem?.ReleaseControllerButton(value);
            }
        }
        
        [Flags]
        enum GamepadButtons : ushort
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
            LB = 0x0100,
            RB = 0x0200
        }

        private readonly Dictionary<GamepadButtons, IController.Keys> _controllerMap = new()
        {
            { GamepadButtons.A, IController.Keys.A },
            { GamepadButtons.X, IController.Keys.B },
            { GamepadButtons.Start, IController.Keys.START },
            { GamepadButtons.Back, IController.Keys.SELECT },
            { GamepadButtons.DPadDown,  IController.Keys.DOWN },
            { GamepadButtons.DPadLeft,  IController.Keys.LEFT },
            { GamepadButtons.DPadRight, IController.Keys.RIGHT },
            { GamepadButtons.DPadUp, IController.Keys.UP }
        };
        
        private readonly Dictionary<Key, IController.Keys> _keyMap = new()
        {
            {Key.A, IController.Keys.A},
            {Key.S, IController.Keys.B},
            {Key.Left, IController.Keys.LEFT},
            {Key.Right, IController.Keys.RIGHT},
            {Key.Up, IController.Keys.UP},
            {Key.Down, IController.Keys.DOWN},
            {Key.Enter, IController.Keys.START},
            {Key.Space, IController.Keys.SELECT}
        };

        protected override async void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (_nesSystem?.Running != true) return;
            
            if (e.Key == Key.Escape)
            {
                await Stop();
                e.Handled = true;
                return;
            }
                
            if (_keyMap.TryGetValue(e.Key, out var value))
            {
                _nesSystem?.PressControllerButton(value);
                e.Handled = true;
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            
            if (_nesSystem?.Running != true) return;
            if (!_keyMap.TryGetValue(e.Key, out var value)) return;
            
            _nesSystem?.ReleaseControllerButton(value);
            e.Handled = true;
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            await Stop();
        }

        private async Task Stop()
        {
            if (_nesSystem?.Running == true)
            {
                await Task.Run(() => _nesSystem?.Stop());
                _logger.Information("Stopped");
                contentPanel.Children.RemoveAt(0);
                contentPanel.Children.Add(this.fileListView);
                SetButtonsFocusable(true);
                this.fileListView.Focus();
                _nesSystem.Apu.SamplesReady -= _audioPlayer.AddSamples;
                _audioPlayer.Clear();
            }
        }
        private const string ScreenshotsDirectory = "screenshots";
        private void Screenshot_OnClick(object sender, RoutedEventArgs e)
        {
            if (_nesSystem?.Running != true) return;
            
            if (!Directory.Exists(ScreenshotsDirectory))
            {
                Directory.CreateDirectory(ScreenshotsDirectory);
            }
            var time = DateTime.Now;
            var filePath = $"{ScreenshotsDirectory}/screenshot_{time:yyyy-MM-dd_HH-mm-ss}.png";
            using var stream = new FileStream(filePath, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_bitmap));
            encoder.Save(stream);
        } 

        private static ILogger CreateLogger()
        {
            if (File.Exists("RobotNES.log"))
            {
                File.Delete("RobotNES.log");
            }

            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo
                .Console()
                .WriteTo.File("RobotNES.log")
                .CreateLogger();

            return logger;
        }

        private void SetButtonsFocusable(bool focus)
        {
            this.StopButton.Focusable = focus;
            this.ScreenshotButton.Focusable = focus;
        }
        
        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var file = (NesFile)((ListViewItem)sender).Content;
                _nesSystem = new NesSystem(file.Path, _logger);
                _nesSystem.FrameBufferReady += RenderFrame;
                _nesSystem.Apu.SamplesReady += _audioPlayer.AddSamples;

                var image = new NearestScalingImage(_bitmap)
                {
                    Focusable = true
                };
                this.contentPanel.Children.Remove(this.fileListView);
                this.contentPanel.Children.Add(image);
                SetButtonsFocusable(false);
                _stopwatch.Start();
                await Task.Run(() => _nesSystem?.Run());
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Unhandled error");
                MessageBox.Show($"Unhandled error: {ex.Message}.\n{ex.StackTrace}", "Error", MessageBoxButton.OK , MessageBoxImage.Error);
                await Stop();
            }
        }

        private void RenderFrame(byte[] frameBuffer)
        {
            Dispatcher.BeginInvoke(() =>
            {
                _bitmap.Lock();
                _bitmap.AddDirtyRect(_bitmapRect);
                var pixels = _bitmap.BackBuffer;
                Marshal.Copy(frameBuffer, 0, pixels, frameBuffer.Length);
                _bitmap.Unlock();

                CountFps();
            });
        }

        private void CountFps()
        {
            _stopwatch.Stop();
            var frametime = _stopwatch.Elapsed.TotalMilliseconds;
            _frameTimes[_frameIndex] = frametime;
            _frameIndex = (_frameIndex + 1) % _frameTimes.Length;
            
            var fps = 1000 / _frameTimes.Average();
            Title = $"RobotNES | {fps} FPS";
            _stopwatch.Restart();
        }
    }
}
