using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace FileRotator
{
    public class Config
    {
        public static ConfigModel LocalConfig;
        private static void DeserializeConfig()
        {
            using (StreamReader file = File.OpenText(Environment.CurrentDirectory + @"\GameData\people.json"))
                LocalConfig = JsonConvert.DeserializeObject<ConfigModel>(file.ReadToEnd());
        }
        private static void SerializeConfig()
        {
            using (StreamWriter file = File.CreateText(Environment.CurrentDirectory + @"\GameData\people.json"))
                file.Write(JsonConvert.SerializeObject(LocalConfig, Formatting.Indented));
        }
    }
    public class ConfigModel
    {
        [JsonProperty] string SourceDir;
        [JsonProperty] string DestDir;
        [JsonProperty] char SplitChar;
        [JsonProperty] bool RemoveSourceFiles;
        [JsonProperty] int LowestLogLevel;
        [JsonProperty] int DailyBackupsToKeep;
        [JsonProperty] int WeeklyBackupsToKeep;
        [JsonProperty] int MonthlyBackupsToKeep;
        [JsonProperty] int YearlyBackupsToKeep;
    }
}
