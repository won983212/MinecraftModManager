using System.Windows.Media.Imaging;

namespace MinecraftModManager.Model
{
    public class Mod
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public string FilePath { get; set; }

        public string LogoPath { get; set; }

        public BitmapImage Thumbnail { get; set; }
    }
}
