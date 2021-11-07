using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Model
{
    public class ModVersionInfo
    {
        public bool IsUpgrade { get; set; } = false;

        public string ModName { get; set; }

        public string ModVersion { get; set; }

        public int CurseForgeID { get; set; }

        public int CurseForgeFileID { get; set; }

        public string CurseForgeURL { get; set; }

        public string LoadError { get; set; }

        public string LatestVersion { get; set; }
    }
}
