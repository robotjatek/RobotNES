using NESCore;

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
        private readonly NesSystem _nesSystem;

        public MainWindow()
        {
            InitializeComponent();

            if(File.Exists("RobotNES.log"))
            {
                File.Delete("RobotNES.log");
            }

            _nesSystem = new NesSystem("nestest.nes");
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => _nesSystem.Run());
        }

        private async void Stop_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => _nesSystem.Stop());
        }
    }
}
