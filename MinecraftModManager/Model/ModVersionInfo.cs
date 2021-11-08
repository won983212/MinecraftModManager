using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Model
{
    public class ModVersionInfo : INotifyPropertyChanged
    {
        private bool _isUpgrade = false;
        private Mod _mod;
        private int _curseForgeId = 0;
        private int _curseForgeFileId = 0;
        private string _curseForgeURL = "";
        private string _loadError = "";
        private string _latestVersion = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsUpgrade
        {
            get => _isUpgrade;
            set => SetPropertyAndNotify(ref _isUpgrade, value);
        }

        public Mod ModObj
        {
            get => _mod;
            set => SetPropertyAndNotify(ref _mod, value);
        }

        public int CurseForgeID
        {
            get => _curseForgeId;
            set => SetPropertyAndNotify(ref _curseForgeId, value);
        }

        public int CurseForgeFileID
        {
            get => _curseForgeFileId;
            set => SetPropertyAndNotify(ref _curseForgeFileId, value);
        }

        public string CurseForgeURL
        {
            get => _curseForgeURL;
            set => SetPropertyAndNotify(ref _curseForgeURL, value);
        }

        public string LoadError
        {
            get => _loadError;
            set => SetPropertyAndNotify(ref _loadError, value);
        }

        public string LatestVersion
        {
            get => _latestVersion;
            set => SetPropertyAndNotify(ref _latestVersion, value);
        }

        public void CopyCurseForgeDataFrom(ModVersionInfo src)
        {
            CurseForgeID = src.CurseForgeID;
            CurseForgeFileID = src.CurseForgeFileID;
            CurseForgeURL = src.CurseForgeURL;
            LoadError = src.LoadError;
            LatestVersion = src.LatestVersion;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetPropertyAndNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            OnPropertyChanged(propertyName);
        }
    }
}
