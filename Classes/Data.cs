using System.Collections.Generic;
using System.Windows.Media;

namespace DeepDiveEmulator.ClassData
{
    public class Data
    {
        public List<DataGameVersion> GameVersions = new List<DataGameVersion>();
        public List<DataDeepDive> DeepDives = new List<DataDeepDive>();
        public List<DataEvent> Events = new List<DataEvent>();
    }
    public class DataGameVersion
    {
        public string IdFake = "";
        public string Name = "";
        public string Path = "";
        public string Manifest = "";
        public Brush Brush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
    }
    public class DataDeepDive
    {
        public string IdFake = "";
        public uint Seed = 0;
        public string GameVersion = "";
        public string Event = "";
        public string Date = "";
        public Brush Brush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
        public DataDeepDiveType Normal = new DataDeepDiveType();
        public DataDeepDiveType Elite = new DataDeepDiveType();
    }
    public class DataDeepDiveType
    {
        public string Tag = "";
        public string Name = "";
        public string Region = "";
        public DataDeepDiveMission[] Missions = new DataDeepDiveMission[3] { new DataDeepDiveMission(), new DataDeepDiveMission(), new DataDeepDiveMission() };
        public DataDeepDiveObjective[] Objectives = new DataDeepDiveObjective[3] { new DataDeepDiveObjective(), new DataDeepDiveObjective(), new DataDeepDiveObjective() };
        public string[] Warnings = new string[3] {"", "", ""};
        public string[] Anomalies = new string[3] {"", "", ""};
    }
    public class DataDeepDiveMission
    {
        public string Type = "";
        public string Value = "";
    }
    public class DataDeepDiveObjective
    {
        public string Type = "";
        public string Value = "";
    }
    public class DataEvent
    {
        public string IdFake = "";
        public string Year = "2018";
        public string Name = "";
        public string Command = "";
        public List<DataEventItem> Items = new List<DataEventItem>();
    }
    public class DataEventItem
    {
        public string Name = "";
        public string Type = "";
    }
}
