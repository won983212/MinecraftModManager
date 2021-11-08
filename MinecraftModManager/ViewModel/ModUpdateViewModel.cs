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
        private readonly ModUpdater _modUpdater = new ModUpdater();
        private string _loadingTitle = "";
        private string _loadingStatus = null;

        public ModVersionInfo SelectedItem { get; set; }
        public ObservableCollection<ModVersionInfo> ModVersionInfos { get; } = new ObservableCollection<ModVersionInfo>();
        public ICommand ListUpdateCommand => new RelayCommand(() => UpdateModVersionListAsync(_mods));
        public ICommand UpgradeModCommand => new RelayCommand(() => UpgradeSelectedMod(ModVersionInfos));
        public ICommand RefreshModCommand => new RelayCommand(() => RefreshModVersionInfo(SelectedItem));
        public ICommand SetProjectIDCommand => new RelayCommand(() => { SetProjectID(SelectedItem); RefreshModVersionInfo(SelectedItem); });

        public string LoadingTitle
        {
            get => _loadingTitle;
            set => SetPropertyAndNotify(ref _loadingTitle, value);
        }

        public string LoadingStatus
        {
            get => _loadingStatus;
            set => SetPropertyAndNotify(ref _loadingStatus, value);
        }

        public ModUpdateViewModel(IEnumerable<Mod> mods)
        {
            if (mods == null)
                throw new InvalidOperationException("mods는 null이 될 수 없습니다.");

            _mods = mods;
            UpdateModVersionListAsync(mods);
        }

        private async Task UpdateModVersionListAsync(IEnumerable<Mod> mods)
        {
            int count = 0;
            List<ModVersionInfo> modInfos = new List<ModVersionInfo>();
            LoadingTitle = "최신 버전 정보를 불러오고 있습니다..";
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

        private async Task UpgradeSelectedMod(IEnumerable<ModVersionInfo> mods)
        {
            int count = 0;
            LoadingTitle = "선택된 모드들을 업그레이드 하고 있습니다..";
            mods = mods.Where((mod) => mod.IsUpgrade);
            foreach (ModVersionInfo mod in mods)
            {
                count++;
                LoadingStatus = mod.ModObj.Name + " 업그레이드 중... (" + count + "/" + mods.Count() + ")";
                await _modUpdater.UpgradeMod(mod);
                mod.IsUpgrade = false;
            }
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
            LoadingStatus = modVersionInfo.ModObj.Name + " 버전 정보 불러오는 중...";
            ModVersionInfo ent = await _versionRetriever.GetModVersionInfoAsync(modVersionInfo);
            modVersionInfo.CopyCurseForgeDataFrom(ent);
            LoadingStatus = null;
        }

        private void SetProjectID(ModVersionInfo modVersionInfo)
        {
            int id = GetProjectID(modVersionInfo);
            string result = ViewUtil.ShowInputDialog(modVersionInfo.ModObj.Name + "의 Project ID를 입력해주세요. (0 입력시 기본값으로)", id.ToString());
            if (!string.IsNullOrWhiteSpace(result))
            {
                if (int.TryParse(result, out int result_id))
                    id = result_id;
            }
            CurseForgeProjectInfoRetriever.SetExplicitProjectId(modVersionInfo.ModObj.Name, id);
            CurseForgeProjectInfoRetriever.SaveProjectIdMap();
        }

        private int GetProjectID(ModVersionInfo modVersionInfo)
        {
            int explicitId = CurseForgeProjectInfoRetriever.GetExplicitProjectId(modVersionInfo.ModObj.Name);
            if (explicitId != 0)
                return explicitId;
            return modVersionInfo.CurseForgeID;
        }
    }
}
