using MinecraftModManager.Model;
using MinecraftModManager.Mods;
using MinecraftModManager.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MinecraftModManager.ViewModel
{
    internal class ModUpdateViewModel : BaseViewModel
    {
        private readonly IEnumerable<Mod> _mods;
        private readonly ModVersionRetriever _versionRetriever = new ModVersionRetriever();
        private string _loadingStatus = null;

        public ModVersionInfo SelectedItem { get; set; }
        public ObservableCollection<ModVersionInfo> ModVersionInfos { get; } = new ObservableCollection<ModVersionInfo>();
        public ICommand ListUpdateCommand => new RelayCommand(() => UpdateModVersionListAsync(_mods));
        public ICommand UpgradeModCommand => new RelayCommand(() => Console.WriteLine("UPDATE!!!!")); // TODO Upgrade all mods
        public ICommand RefreshModCommand => new RelayCommand(() => RefreshModVersionInfo(SelectedItem));
        public ICommand SetProjectIDCommand => new RelayCommand(() => SetProjectID(SelectedItem));

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
            LoadingStatus = "";
            foreach (Mod mod in mods)
            {
                count++;
                LoadingStatus = mod.Name + " 버전 정보 불러오는 중... (" + count + "/" + mods.Count() + ")";
                ModVersionInfo ent = await _versionRetriever.GetModVersionInfoAsync(mod);
                modInfos.Add(ent);
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

        private async Task RefreshModVersionInfo(ModVersionInfo modVersionInfo)
        {
            LoadingStatus = modVersionInfo.ModName + " 버전 정보 불러오는 중...";
            ModVersionInfo ent = await _versionRetriever.GetModVersionInfoAsync(modVersionInfo);
            modVersionInfo.CopyCurseForgeDataFrom(ent);
            LoadingStatus = null;
        }

        private void SetProjectID(ModVersionInfo modVersionInfo)
        {
            int id = GetProjectID(modVersionInfo);
            string result = ViewUtil.ShowInputDialog(modVersionInfo.ModName + "의 Project ID를 입력해주세요. (0 입력시 기본값으로)", id.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (int.TryParse(result, out int result_id))
                    id = result_id;
            }
            CurseForgeProjectInfoRetriever.SetExplicitProjectId(modVersionInfo.ModName, id);
            CurseForgeProjectInfoRetriever.SaveProjectIdMap();
        }

        private int GetProjectID(ModVersionInfo modVersionInfo)
        {
            int explicitId = CurseForgeProjectInfoRetriever.GetExplicitProjectId(modVersionInfo.ModName);
            if (explicitId != 0)
                return explicitId;
            return modVersionInfo.CurseForgeID;
        }
    }
}
