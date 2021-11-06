using MinecraftModManager.Model;
using MinecraftModManager.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.ViewModel
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public ObservableCollection<Mod> Mods { get; private set; } = new ObservableCollection<Mod>();

        public MainWindowViewModel()
        {
            Mods.Add(new Mod() { Name = "Pehkui", Description = "Allows resizing of most entities.", Version = "2.2.1+1.16.5-forge" });
            Mods.Add(new Mod() { Name = "Macaw's Doors", Description = "Adds a lot of new Doors! With vanila and unique styles.", Version = "1.0.3" });
            Mods.Add(new Mod() { Name = "AutoRegLib", Description = "Automatically item, block, and model registration for mods.", Version = "1.6-49" });
            LoadModList();
        }

        public void LoadModList()
        {
            string selectedPath = FileSystemUtil.SelectDirectory("mods폴더 지정");
            if (selectedPath == null)
                return;
            Mods.Clear();
            AddModsFromDirectory(selectedPath);
        }

        private void AddModsFromDirectory(string dirPath)
        {
            foreach (string file in Directory.EnumerateFiles(dirPath))
            {
                if (!file.EndsWith(".jar"))
                    continue;
                Mod mod = ParseModFromJar(file);
                if (mod != null)
                    Mods.Add(mod);
            }
        }

        private Mod ParseModFromJar(string filePath)
        {
            Logger.Debug("Open " + filePath);
            Mod result = null;
            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                ZipArchiveEntry modsTomlEntry = archive.GetEntry("META-INF/mods.toml");
                if (modsTomlEntry == null)
                    return null;
                using (Stream stream = modsTomlEntry.Open())
                {
                    StreamReader streamReader = new StreamReader(stream);
                    result = ParseModFromTomlStream(streamReader);
                }
                if (result.LogoPath != null)
                    result.Thumbnail = ReadLogoImage(archive, result.LogoPath);
                if (result.Version == "${file.jarVersion}")
                    result.Version = GetJarVersion(archive);
                if (result.Version == null)
                    result.Version = Path.GetFileName(filePath);
            }
            return result;
        }

        private BitmapImage ReadLogoImage(ZipArchive archive, string logoFile)
        {
            ZipArchiveEntry logoFileEntry = archive.GetEntry(logoFile);
            if (logoFileEntry == null)
                return null;
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = logoFileEntry.Open();
            image.EndInit();
            return image;
        }

        private string GetJarVersion(ZipArchive archive)
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
            Dictionary<string, string> properties = ParseToml(streamReader);
            if (!properties.ContainsKey("modId"))
                return null;
            if (!properties.ContainsKey("version"))
                return null;
            Mod mod = new Mod();
            mod.Name = properties["modId"];
            mod.Version = properties["version"];
            if (properties.TryGetValue("description", out string description))
                mod.Description = description;
            if (properties.TryGetValue("logoFile", out string logoFile))
                mod.LogoPath = logoFile;
            return mod;
        }

        private Dictionary<string, string> ParseToml(StreamReader streamReader)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            string line;
            string continuedStringPropertyName = null;
            ReadToNextModsCategory(streamReader);
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#"))
                    continue;
                string categoryName = ExtractCategoryName(line);
                if (categoryName != null && categoryName != "mods")
                    break;
                if (continuedStringPropertyName != null)
                {
                    properties[continuedStringPropertyName] += EliminateLiteralMark(line);
                    if (ContainsMultiLineEnd(line))
                        continuedStringPropertyName = null;
                    continue;
                }
                int separatorIndex = line.IndexOf('=');
                if (separatorIndex == -1)
                    continue;
                string propertyName = line.Substring(0, separatorIndex).Trim();
                string propertyValue = line.Substring(separatorIndex + 1).Trim();
                if (ContainsMultiLineStart(propertyValue))
                    continuedStringPropertyName = propertyName;
                propertyValue = EliminateLiteralMark(propertyValue);
                properties.Add(propertyName, propertyValue);
            }
            return properties;
        }

        private void ReadToNextModsCategory(StreamReader streamReader)
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (ExtractCategoryName(line) == "mods")
                    break;
            }
        }

        private string ExtractCategoryName(string line)
        {
            if (line.StartsWith("[["))
            {
                int endBracketIndex = line.IndexOf("]]");
                if (endBracketIndex == -1)
                    return null;
                return line.Substring(2, endBracketIndex - 2);
            }
            return null;
        }

        private string EliminateLiteralMark(string value)
        {
            if (value.Contains("#"))
                value = value.Substring(0, value.IndexOf('#'));
            value = value.Trim('\"', '\'', ' ', '\t');
            return value;
        }

        private bool ContainsMultiLineStart(string line)
        {
            return line.StartsWith("\"\"\"") || line.StartsWith("\'\'\'");
        }

        private bool ContainsMultiLineEnd(string line)
        {
            return line.EndsWith("\"\"\"") || line.EndsWith("\'\'\'");
        }
    }
}
