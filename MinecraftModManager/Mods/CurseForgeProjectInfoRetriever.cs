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
            string json = await GetJsonFromWeb(url);
            return json;
        }

        private static async Task<string> GetJsonFromWeb(string url)
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

        private static string GetModInfoURL(string modName)
        {
            return "https://addons-ecs.forgesvc.net/api/v2/addon/search?gameId=432&index=0&sectionId=6&sort=0&pageSize=1&searchFilter=" + modName;
        }

        private static string GetModInfoURLFromID(int projectID)
        {
            return "https://addons-ecs.forgesvc.net/api/v2/addon/" + projectID;
        }

        private static void LoadProjectIdMap()
        {
            string projectIdJson = ProjectProperties.Get<string>(PropertyKeys.ProjectIDMap);
            if (string.IsNullOrWhiteSpace(projectIdJson))
            {
                _projectIdMap = new Dictionary<string, int>();
                return;
            }
            _projectIdMap = JsonConvert.DeserializeObject<Dictionary<string, int>>(projectIdJson);
        }

        public static int GetExplicitProjectId(string modName)
        {
            if (_projectIdMap.TryGetValue(modName, out int id))
                return id;
            return 0;
        }

        public static void SetExplicitProjectId(string modName, int id)
        {
            if (id <= 0)
            {
                _projectIdMap.Remove(modName);
                return;
            }
            _projectIdMap[modName] = id;
        }

        public static void SaveProjectIdMap()
        {
            ProjectProperties.Set(PropertyKeys.ProjectIDMap, JsonConvert.SerializeObject(_projectIdMap));
            ProjectProperties.Save();
        }

        private static async Task<string> GetJsonFromFile(string modName)
        {
            string path = @"../../../dummyJson/" + modName.Replace(" ", "_").Replace(":", "-") + ".json";
            using (var reader = File.OpenText(path))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
