using MinecraftModManager.Model;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.Mods
{
    public class ModJarInfoReader
    {
        private string filePath;
        private ZipArchive archive = null;
        private readonly ModsTomlParser tomlParser = new ModsTomlParser();

        public Mod ParseModFromJar(string filePath)
        {
            Logger.Debug("Open " + filePath);
            this.filePath = filePath;
            Mod result = null;
            using (archive = ZipFile.OpenRead(filePath))
            {
                ZipArchiveEntry modsTomlEntry = archive.GetEntry("META-INF/mods.toml");
                if (modsTomlEntry == null)
                    return null;
                using (Stream stream = modsTomlEntry.Open())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    result = ParseModFromTomlStream(streamReader);
                    result.FilePath = filePath;
                }
                FillVersionAndThumbnail(result);
            }
            archive = null;
            return result;
        }

        private void FillVersionAndThumbnail(Mod mod)
        {
            if (mod == null)
                return;
            if (mod.LogoPath != null)
                mod.Thumbnail = CreateLogoImage(mod.LogoPath);
            if (mod.Version == "${file.jarVersion}")
                mod.Version = GetJarVersion();
            if (mod.Version == null)
                mod.Version = Path.GetFileName(filePath);
        }

        private BitmapImage CreateLogoImage(string logoFile)
        {
            ZipArchiveEntry logoFileEntry = archive.GetEntry(logoFile);
            if (logoFileEntry == null)
                return null;
            BitmapImage image;
            using (MemoryStream stream = new MemoryStream())
            {
                using (Stream imageStream = logoFileEntry.Open())
                {
                    imageStream.CopyTo(stream);
                }
                image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }

        private string GetJarVersion()
        {
            ZipArchiveEntry manifestEntry = archive.GetEntry("META-INF/MANIFEST.MF");
            if (manifestEntry == null)
                return null;
            using (Stream stream = manifestEntry.Open())
            {
                StreamReader streamReader = new StreamReader(stream);
                return ReadManifestVersion(streamReader);
            }
        }

        private string ReadManifestVersion(StreamReader streamReader)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                int separatorIndex = line.IndexOf(':');
                if (separatorIndex == -1)
                    continue;
                string propertyKey = line.Substring(0, separatorIndex).Trim();
                string propertyValue = line.Substring(separatorIndex + 1).Trim();
                if (propertyKey == "Implementation-Version")
                    return propertyValue;
            }
            return null;
        }

        private Mod ParseModFromTomlStream(StreamReader streamReader)
        {
            Dictionary<string, string> properties = tomlParser.ParseToml(streamReader);
            if (!ValidateTomlProperty(properties))
                return null;

            Mod mod = new Mod();
            mod.Name = properties["displayName"];
            mod.Version = properties["version"];
            if (properties.TryGetValue("description", out string description))
                mod.Description = description;
            if (properties.TryGetValue("logoFile", out string logoFile))
                mod.LogoPath = logoFile;
            return mod;
        }

        private bool ValidateTomlProperty(Dictionary<string, string> properties)
        {
            if (!properties.ContainsKey("displayName"))
                return false;
            if (!properties.ContainsKey("version"))
                return false;
            return true;
        }
    }
}
