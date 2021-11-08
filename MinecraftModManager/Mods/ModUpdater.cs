using MinecraftModManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Mods
{
    public class ModUpdater
    {
        public async Task UpgradeMod(ModVersionInfo mod)
        {
            Logger.Debug("Upgrade Mod: " + mod.ModObj.Name);
            string url = await GetDownloadURL(mod);

            Logger.Debug("Download Mod: " + url);
            string tempFilePath = await DownloadFile(mod, url);

            string targetPath = Path.Combine(Path.GetDirectoryName(mod.ModObj.FilePath), mod.LatestVersion);
            File.Delete(mod.ModObj.FilePath);
            File.Move(tempFilePath, targetPath);

            string version = await Task.Run(() => GetModVersionFromJar(targetPath));
            mod.ModObj.Version = version;
        }

        private async Task<string> GetDownloadURL(ModVersionInfo mod)
        {
            string url = "https://addons-ecs.forgesvc.net/api/v2/addon/" + mod.CurseForgeID + "/file/" + mod.CurseForgeFileID + "/download-url";
            using (WebClient client = new WebClient())
            {
                return await client.DownloadStringTaskAsync(new Uri(url));
            }
        }

        private async Task<string> DownloadFile(ModVersionInfo mod, string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 5000;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("HTTP response status code is not OK. URL: " + url + ", Code: " + response.StatusCode);
                }
                string tempFilePath = Path.GetTempFileName();
                byte[] buffer = new byte[1024];
                using (FileStream fileStream = File.OpenWrite(tempFilePath))
                using (Stream respStream = response.GetResponseStream())
                {
                    await respStream.CopyToAsync(fileStream);
                }
                return tempFilePath;
            }
        }

        private string GetModVersionFromJar(string path)
        {
            ModJarInfoReader reader = new ModJarInfoReader();
            Mod mod = reader.ParseModFromJar(path);
            return mod.Version;
        }
    }
}
