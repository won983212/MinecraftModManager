using MinecraftModManager.Model;
using MinecraftModManager.Mods;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinecraftModManager.ViewModel
{
    internal class ModUpdateViewModel : BaseViewModel
    {
        private readonly IEnumerable<Mod> _mods;

        public ObservableCollection<ModVersionInfo> ModVersionInfos { get; } = new ObservableCollection<ModVersionInfo>();
        public ICommand ListUpdateCommand => new RelayCommand(() => UpdateModVersionListAsync(_mods));
        public ICommand UpgradeModCommand => new RelayCommand(() => Console.WriteLine("UPDATE!!!!")); // TODO Upgrade all mods

        private string _loadingStatus = null;
        public string LoadingStatus
        {
            get => _loadingStatus;
            set => SetPropertyAndNotify(ref _loadingStatus, value);
        }

        public ModUpdateViewModel(IEnumerable<Mod> mods)
        {
            _mods = mods;
            UpdateModVersionListAsync(mods);
        }

        private async Task UpdateModVersionListAsync(IEnumerable<Mod> mods)
        {
            if (mods == null)
                return;

            int count = 0;
            List<ModVersionInfo> modInfos = new List<ModVersionInfo>();
            ModVersionRetriever versionRetriever = new ModVersionRetriever();
            LoadingStatus = "";
            foreach (Mod mod in mods)
            {
                count++;
                LoadingStatus = mod.Name + " 버전 정보 불러오는 중... (" + count + "/" + mods.Count() + ")";
                ModVersionInfo ent = await versionRetriever.GetModVersionInfoAsync(mod);
                if (ent != null)
                {
                    modInfos.Add(ent);
                }
            }
            SetToModVersionInfo(modInfos);
            LoadingStatus = null;
        }

        private void SetToModVersionInfo(IEnumerable<ModVersionInfo> modInfos)
        {
            ModVersionInfos.Clear();
            foreach (ModVersionInfo ent in modInfos)
            {
                ModVersionInfos.Add(ent);
            }
        }
    }
}
