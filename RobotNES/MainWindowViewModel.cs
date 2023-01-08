using System;
using System.Collections.ObjectModel;
using System.IO;

namespace RobotNES
{
    public class NesFile
    {
        public string Path { get; init; }

        public string Name { get; init; }

        public NesFile(string path, string name)
        {
            Path = path;
            Name = name;
        }
    }

    public class MainWindowViewModel
    {
        public ObservableCollection<NesFile> NesFiles { get; private set; } = new ObservableCollection<NesFile>();

        public MainWindowViewModel()
        {
            var romsPath = Path.Combine(Environment.CurrentDirectory, "ROMs");
            if (Directory.Exists(romsPath))
            {
                var directory = new DirectoryInfo(romsPath);
                var nesFiles = directory.GetFiles("*.nes", SearchOption.TopDirectoryOnly);
                foreach (var nesFile in nesFiles)
                {
                    NesFiles.Add(new NesFile(nesFile.FullName, nesFile.Name));
                }
            }
        }

    }
}
