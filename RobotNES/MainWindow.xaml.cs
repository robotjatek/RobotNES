using NESCore;

using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotNES
{
    public partial class MainWindow : Window
    {
        private NesSystem? _nesSystem;
        private readonly ILogger _logger;

        public MainWindow()
        {
            InitializeComponent();
            _logger = CreateLogger();
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => _nesSystem?.Stop());
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
        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var file = (NesFile)(((ListViewItem)sender).Content);
                _nesSystem = new NesSystem(file.Path, _logger);
                await Task.Run(() => _nesSystem?.Run());
            }
            catch (Exception ex)
            {
                // log any uncatched errors
                _logger.Fatal(ex, "Unhandled error.");
            }
        }
    }
}
