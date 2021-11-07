using MinecraftModManager.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Mods
{
    public class CurseForgeProjectInfoRetriever
    {
        private static Dictionary<string, int> _projectIdMap = null;

        public async Task<string> GetModProjectInfoJson(string modName)
        {
            string url = GetModInfoURL(modName);
            if (_projectIdMap == null)
                LoadProjectIdMap();
            if (_projectIdMap.TryGetValue(modName, out int id))
                url = GetModInfoURLFromID(id);
            return await GetJsonFromWeb(url);
        }

        private async Task<string> GetJsonFromWeb(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.Error("HTTP response status code is not OK. URL: " + url + ", Code: " + response.StatusCode);
                }
                using (StreamReader responseStream = new StreamReader(response.GetResponseStream()))
                {
                    return responseStream.ReadToEnd();
                }
            }
        }

        private string GetModInfoURL(string modName)
        {
            return "https://addons-ecs.forgesvc.net/api/v2/addon/search?gameId=432&index=0&sectionId=6&sort=0&pageSize=1&searchFilter=" + modName;
        }

        private string GetModInfoURLFromID(int projectID)
        {
            return "https://addons-ecs.forgesvc.net/api/v2/addon/" + projectID;
        }

        private static void LoadProjectIdMap()
        {
            string projectIdJson = Settings.Default.ProjectIDMap;
            if (string.IsNullOrWhiteSpace(projectIdJson))
            {
                _projectIdMap = new Dictionary<string, int>();
                return;
            }
            _projectIdMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(projectIdJson);
        }

        public static void AddExplicitProjectId(string modName, int id)
        {
            _projectIdMap.Add(modName, id);
        }

        public static void SaveProjectIdMap()
        {
            Settings.Default.ProjectIDMap = JsonConvert.SerializeObject(_projectIdMap);
            Settings.Default.Save();
        }
    }
}
