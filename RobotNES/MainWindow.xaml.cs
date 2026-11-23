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
using RobotNES.Settings;

namespace RobotNES
{
    public partial class MainWindow
    {
        private readonly Stopwatch _stopwatch = new();
        private int _frameIndex;
        private readonly double[] _frameTimes = new double[60];

        private NesSystem? _nesSystem;
        private readonly ILogger _logger;
        private readonly WriteableBitmap _bitmap = new(256, 240, 96.0, 96.0, PixelFormats.Bgr24, null);
        private readonly Int32Rect _bitmapRect = new(0, 0, 256, 240);
        private readonly AudioPlayer _audioPlayer = new();

        private NearestScalingImage? _image;
        private readonly GamepadListener _gamepadListener = new();

        public MainWindow()
        {
            InitializeComponent();
            _logger = CreateLogger();
            _gamepadListener.ButtonDown += ButtonDown;
            _gamepadListener.ButtonUp += ButtonUp;
            
            _gamepadListener.Start();
        }

        
        private void ButtonDown(GamepadListener.GamepadButtons button)
        {
            if (ControllerMappings.Instance.ControllerMap.TryGetValue(button, out var value))
            {
                _nesSystem?.PressControllerButton(value);
            }
        }

        private void ButtonUp(GamepadListener.GamepadButtons button)
        {
            if (ControllerMappings.Instance.ControllerMap.TryGetValue(button, out var value))
            {
                _nesSystem?.ReleaseControllerButton(value);
            }
        }
        
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
            if (_nesSystem == null)
                return;

            await Task.Run(() => _nesSystem.Stop());
            _nesSystem = null;
            _logger.Information("Stopped");

            contentPanel.Children.Remove(_image);
            contentPanel.Children.Add(this.fileListView);
            SetButtonsFocusable(true);
            this.fileListView.Focus();
            _audioPlayer.Clear();
        }

        private async Task Resume()
        {
            if (_nesSystem is not { Running: false })
                return;

            await Task.Run(() => _nesSystem?.Run());
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

                _image = new NearestScalingImage(_bitmap)
                {
                    Focusable = true
                };
                this.contentPanel.Children.Remove(this.fileListView);
                this.contentPanel.Children.Add(_image);
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

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            _gamepadListener.Stop();

            if (_nesSystem?.Running == true)
                _nesSystem.Stop();
        }

        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            
            _gamepadListener.Start();
            await Resume();
        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs args)
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }
    }
}
