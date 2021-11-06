using MinecraftModManager.Model;
using MinecraftModManager.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<Mod> Mods { get; private set; } = new ObservableCollection<Mod>();

        public MainWindowViewModel()
        {
            Mods.Add(new Mod() { Name = "Pehkui", Description = "Allows resizing of most entities.", Version = "2.2.1+1.16.5-forge" });
            Mods.Add(new Mod() { Name = "Macaw's Doors", Description = "Adds a lot of new Doors! With vanila and unique styles.", Version = "1.0.3" });
            Mods.Add(new Mod() { Name = "AutoRegLib", Description = "Automatically item, block, and model registration for mods.", Version = "1.6-49" });
            LoadModList();
        }

        public void LoadModList()
        {
            string selectedPath = FileSystemUtil.SelectDirectory("mods폴더 지정");
            if (selectedPath == null)
                return;
            Mods.Clear();
            AddModsFromDirectory(selectedPath);
        }

        private void AddModsFromDirectory(string dirPath)
        {
            ModJarInfoReader infoReader = new ModJarInfoReader();
            foreach (string file in Directory.EnumerateFiles(dirPath))
            {
                if (!file.EndsWith(".jar"))
                    continue;
                Mod mod = infoReader.ParseModFromJar(file);
                if (mod != null)
                    Mods.Add(mod);
            }
        }
    }
}
