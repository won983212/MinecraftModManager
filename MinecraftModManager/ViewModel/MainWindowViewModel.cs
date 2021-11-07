using MinecraftModManager.Model;
using MinecraftModManager.Mods;
using MinecraftModManager.Properties;
using MinecraftModManager.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinecraftModManager.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<Mod> Mods { get; } = new ObservableCollection<Mod>();
        public ICommand ModsRefreshCommand => new RelayCommand(() => LoadModListAsync());
        public ICommand ModsUpdateCommand => new RelayCommand(() => ViewUtil.ShowWindow(new ModUpdateViewModel(Mods)));

        private bool _isLoadingModList = false;
        public bool IsLoadingModList
        {
            get => _isLoadingModList;
            set => SetPropertyAndNotify(ref _isLoadingModList, value);
        }

        public MainWindowViewModel()
        {
            LoadModListAsync();
        }

        public async Task LoadModListAsync()
        {
            if (IsLoadingModList)
                return;
            IsLoadingModList = true;
            string modsPath = Settings.Default.ModsDirectory;
            if (!Directory.Exists(modsPath))
            {
                modsPath = FileSystemUtil.SelectDirectory("mods폴더 지정");
                if (modsPath == null)
                    return;
                Settings.Default.ModsDirectory = modsPath;
                Settings.Default.Save();
            }
            List<Mod> mods = await Task.Run(() => GetModsInfoFromDirectory(modsPath));
            SetToModsList(mods);
            IsLoadingModList = false;
        }

        private void SetToModsList(IEnumerable<Mod> mods)
        {
            Mods.Clear();
            foreach (Mod mod in mods)
                Mods.Add(mod);
        }

        private List<Mod> GetModsInfoFromDirectory(string dirPath)
        {
            List<Mod> mods = new List<Mod>();
            ModJarInfoReader infoReader = new ModJarInfoReader();
            foreach (string file in Directory.EnumerateFiles(dirPath))
            {
                if (!file.EndsWith(".jar"))
                    continue;
                Mod mod = infoReader.ParseModFromJar(file);
                if (mod != null)
                    mods.Add(mod);
            }
            return mods;
        }
    }
}
