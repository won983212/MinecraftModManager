using System.Collections.Generic;
using System.IO;

namespace MinecraftModManager.Mods
{
    public class ModsTomlParser
    {
        private string line;
        private Dictionary<string, string> properties;
        private string continuedStringPropertyName = null;

        public Dictionary<string, string> ParseToml(StreamReader streamReader)
        {
            Reset();
            ReadToNextModsCategory(streamReader);
            while ((line = streamReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("#"))
                    continue;
                if (!IsModsCategory())
                    break;
                if (AppendContinuedString())
                    continue;
                int separatorIndex = line.IndexOf('=');
                if (separatorIndex == -1)
                    continue;
                AddProperty(separatorIndex);
            }
            return properties;
        }

        private void Reset()
        {
            line = null;
            properties = new Dictionary<string, string>();
            continuedStringPropertyName = null;
        }

        private bool IsModsCategory()
        {
            string categoryName = ExtractCategoryName(line);
            return categoryName == null || categoryName == "mods";
        }

        private bool AppendContinuedString()
        {
            if (continuedStringPropertyName != null)
            {
                string prettyLine = EliminateLiteralMark(line);
                prettyLine = EliminateColorCode(prettyLine);
                properties[continuedStringPropertyName] += prettyLine;
                if (ContainsMultiLineEnd(line))
                    continuedStringPropertyName = null;
                return true;
            }
            return false;
        }

        private void AddProperty(int separatorIndex)
        {
            string propertyName = line.Substring(0, separatorIndex).Trim();
            string propertyValue = line.Substring(separatorIndex + 1).Trim();
            if (ContainsMultiLineStart(propertyValue))
                continuedStringPropertyName = propertyName;
            propertyValue = EliminateLiteralMark(propertyValue);
            propertyValue = EliminateColorCode(propertyValue);
            properties.Add(propertyName, propertyValue);
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

        private static string ExtractCategoryName(string line)
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

        private static bool ContainsMultiLineStart(string line)
        {
            return line.StartsWith("\"\"\"") || line.StartsWith("\'\'\'");
        }

        private static bool ContainsMultiLineEnd(string line)
        {
            return line.EndsWith("\"\"\"") || line.EndsWith("\'\'\'");
        }

        private static string EliminateLiteralMark(string value)
        {
            if (value.Contains("#"))
                value = value.Substring(0, value.IndexOf('#'));
            value = value.Trim('\"', '\'', ' ', '\t');
            return value;
        }

        private static string EliminateColorCode(string value)
        {
            int idx;
            while ((idx = value.IndexOf('§')) != -1)
                value = value.Remove(idx, 2);
            return value;
        }
    }
}
