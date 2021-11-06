using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.Model
{
    public class Mod
    {
        public string Name { get; set; } = null;

        public string Version { get; set; } = null;

        public string Description { get; set; } = null;

        public string LogoPath { get; set; } = null;

        public BitmapImage Thumbnail { get; set; } = null;
    }
}
