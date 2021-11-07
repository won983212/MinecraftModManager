using MinecraftModManager.Model;
using MinecraftModManager.Properties;
using MinecraftModManager.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<Mod> Mods { get; private set; } = new ObservableCollection<Mod>();
        public ICommand ModsRefreshCommand => new RelayCommand(LoadModList);

        public MainWindowViewModel()
        {
            // TODO Async 모델로 바꿔야 할듯?
            LoadModList();
        }

        public void LoadModList()
        { 
            string modsPath = Settings.Default.ModsDirectory;
            if (!Directory.Exists(modsPath))
            {
                modsPath = FileSystemUtil.SelectDirectory("mods폴더 지정");
                if (modsPath == null)
                    return;
                Settings.Default.ModsDirectory = modsPath;
                Settings.Default.Save();
            }
            Mods.Clear();
            AddModsFromDirectory(modsPath);
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
