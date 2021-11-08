using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager
{
    public class PropertyKeys
    {
        public const int PropertyKeySize = 2;
        public static readonly string ProjectIDMap = "ProjectIDMap";
        public static readonly string ModsDirectory = "ModsDirectory";
    }

    public static class ProjectProperties
    {
        private static bool _isLoaded = false;
        private static readonly string _propertyFile = "properties.json";
        private static Dictionary<string, object> _data = new Dictionary<string, object>();

        public static void Reset()
        {
            _data.Clear();
            _data.Add(PropertyKeys.ProjectIDMap, "");
            _data.Add(PropertyKeys.ModsDirectory, "");
            Save();
        }

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(_data);
            File.WriteAllText(_propertyFile, json);
            Logger.Debug("Properties Saved.");
        }

        public static void Load()
        {
            if (!File.Exists(_propertyFile))
                Reset();
            GetAndParseProperties();
            if (_data.Count != PropertyKeys.PropertyKeySize)
            {
                Logger.Error("설정 파일이 잘못되었습니다.");
                Reset();
                GetAndParseProperties();
            }
            _isLoaded = true;
            Logger.Debug("Properties Loaded.");
        }

        private static void GetAndParseProperties()
        {
            string json = File.ReadAllText(_propertyFile);
            _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public static T Get<T>(string key)
        {
            if (!_isLoaded)
                Load();
            return (T)_data[key];
        }

        public static void Set<T>(string key, T value)
        {
            _data[key] = value;
        }
    }
}
