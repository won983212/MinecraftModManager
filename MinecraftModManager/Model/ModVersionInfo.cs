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
        public bool IsUpgrade { get; set; } = false;

        public string ModName { get; set; }

        public string ModVersion { get; set; }

        public int CurseForgeID { get; set; }

        public int CurseForgeFileID { get; set; }

        public string CurseForgeURL { get; set; }

        public string LoadError { get; set; }

        public string LatestVersion { get; set; }


        public void CopyCurseForgeDataFrom(ModVersionInfo src)
        {
            CurseForgeID = src.CurseForgeID;
            CurseForgeFileID = src.CurseForgeFileID;
            CurseForgeURL = src.CurseForgeURL;
            LoadError = src.LoadError;
            LatestVersion = src.LatestVersion;

            OnPropertyChanged(nameof(CurseForgeID));
            OnPropertyChanged(nameof(CurseForgeFileID));
            OnPropertyChanged(nameof(CurseForgeURL));
            OnPropertyChanged(nameof(LoadError));
            OnPropertyChanged(nameof(LatestVersion));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
