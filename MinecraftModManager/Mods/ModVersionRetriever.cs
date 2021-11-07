using MinecraftModManager.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftModManager.Mods
{
    public class ModVersionRetriever
    {
        private static CurseForgeProjectInfoRetriever _infoRetriever = new CurseForgeProjectInfoRetriever();
        private string _minecraftVersion = "1.16.x"; // TODO Change 할 수 있도록 변경

        public async Task<ModVersionInfo> GetModVersionInfoAsync(Mod mod)
        {
            Logger.Debug("Load info " + mod.Name);
            ModVersionInfo modInfo = await FillInfoFromCurseForge(mod.Name);
            modInfo.ModName = mod.Name;
            modInfo.ModVersion = mod.Version;
            return modInfo;
        }

        private async Task<ModVersionInfo> FillInfoFromCurseForge(string modName)
        {
            try
            {
                string json = await _infoRetriever.GetModProjectInfoJson(modName);
                return await Task.Run(() => ParseModInfoJson(json));
            } 
            catch (Exception e)
            {
                ModVersionInfo dummyInfo = new ModVersionInfo();
                dummyInfo.LoadError = e.Message;
                return dummyInfo;
            }
        }

        private ModVersionInfo ParseModInfoJson(string json)
        {
            JArray data = JArray.Parse(json);
            if (data == null || data.Count == 0)
                throw new InvalidOperationException("CurseForge에서 모드를 찾을 수 없습니다.");

            JObject token = data[0] as JObject;
            if (token == null)
                throw new InvalidOperationException("Json format이 [{~~},{~~},...]형식이 아닙니다.");

            JArray latestFiles = token.Value<JArray>("gameVersionLatestFiles");
            JObject latestFile = FindLatestFile(latestFiles);

            ModVersionInfo modInfo = new ModVersionInfo();
            modInfo.CurseForgeID = token.Value<int>("id");
            modInfo.CurseForgeURL = token.Value<string>("websiteUrl");
            modInfo.CurseForgeFileID = latestFile.Value<int>("projectFileId");
            modInfo.LatestVersion = latestFile.Value<string>("projectFileName");
            return modInfo;
        }

        private JObject FindLatestFile(JArray list)
        {
            bool isEndOfWildCardVersion = _minecraftVersion.EndsWith("x");
            foreach (JObject ent in list)
            {
                string gameVersion = ent.Value<string>("gameVersion");
                if (string.IsNullOrWhiteSpace(gameVersion))
                {
                    continue;
                }
                if (isEndOfWildCardVersion)
                {
                    if (gameVersion.StartsWith(_minecraftVersion.Replace("x", "")))
                    {
                        return ent;
                    }
                }
                else if (gameVersion == _minecraftVersion)
                {
                    return ent;
                }
            }
            throw new InvalidOperationException("마인크래프트 버전 " + _minecraftVersion + "에 맞는 최신 모드 파일을 찾을 수 없습니다.");
        }
    }
}
