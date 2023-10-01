using System.Collections.Generic;
using System.Windows.Media;

namespace DeepDiveEmulator.Classes
{
    public class Data
    {
        private DataDiveParam _DiveParameters = new DataDiveParam();
        private List<DataVersion> _Versions = new List<DataVersion>();
        private DataVersionParam _VersionParameters = new DataVersionParam();
        private DataURLs _URLs = new DataURLs();

        public DataDiveParam DiveParameters { get { return _DiveParameters; } set { _DiveParameters = value; } }
        public List<DataVersion> Versions { get { return _Versions; } set { _Versions = value; } }
        public DataVersionParam VersionParameters { get { return _VersionParameters; } set { _VersionParameters = value; } }
        public DataURLs URLs { get { return _URLs; } set { _URLs = value; } }
    }
    public class DataVersion
    {
        private string _Number = "";
        private string _Name = "";
        private string _Path = "";
        private string _Manifest = "";
        private Brush _Brush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
        private List<DataMod> _Mods = new List<DataMod>();
        private List<DataDive> _Dives = new List<DataDive>();
        private List<DataEvent> _Events = new List<DataEvent>();

        public string Number { get { return _Number; } set { _Number = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Path { get { return _Path; } set { _Path = value; } }
        public string Manifest { get { return _Manifest; } set { _Manifest = value; } }
        public Brush Brush { get { return _Brush; } set { _Brush = value; } }
        public List<DataMod> Mods { get { return _Mods; } set { _Mods = value; } }
        public List<DataDive> Dives { get { return _Dives; } set { _Dives = value; } }
        public List<DataEvent> Events { get { return _Events; } set { _Events = value; } }
    }
    public class DataMod
    {
        private string _Number = "";
        private string _Name = "";
        private int _Time = 0;
        private string _Description = "";
        private bool _IsEnabled = false;
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public string Number { get { return _Number; } set { _Number = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public int Time { get { return _Time; } set { _Time = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
    }
    public class DataDive
    {
        private string _Number = "";
        private uint _Seed = 0;
        private int _EventId = -1;
        private string _EventNumber = "";
        private string _Date = "";
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private DataDiveType _Normal = new DataDiveType();
        private DataDiveType _Elite = new DataDiveType();

        public string Number { get { return _Number; } set { _Number = value; } }
        public uint Seed { get { return _Seed; } set { _Seed = value; } }
        public int EventId { get { return _EventId; } set { _EventId = value; } }
        public string EventNumber { get { return _EventNumber; } set { _EventNumber = value; } }
        public string Date { get { return _Date; } set { _Date = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
        public DataDiveType Normal { get { return _Normal; } set { _Normal = value; } }
        public DataDiveType Elite { get { return _Elite; } set { _Elite = value; } }
    }
    public class DataDiveType
    {
        private string _Name = "";
        private string _Region = "";
        private DataDiveMission[] _Missions = new DataDiveMission[3] { new DataDiveMission(), new DataDiveMission(), new DataDiveMission() };
        private DataDiveObjective[] _Objectives = new DataDiveObjective[3] { new DataDiveObjective(), new DataDiveObjective(), new DataDiveObjective() };
        private string[] _Warnings = new string[3] { "", "", "" };
        private string[] _Anomalies = new string[3] { "", "", "" };

        public string Name { get { return _Name; } set { _Name = value; } }
        public string Region { get { return _Region; } set { _Region = value; } }
        public DataDiveMission[] Missions { get { return _Missions; } set { _Missions = value; } }
        public DataDiveObjective[] Objectives { get { return _Objectives; } set { _Objectives = value; } }
        public string[] Warnings { get { return _Warnings; } set { _Warnings = value; } }
        public string[] Anomalies { get { return _Anomalies; } set { _Anomalies = value; } }
    }
    public class DataDiveMission
    {
        private string _Type = "";
        private string _Value = "";

        public string Type { get { return _Type; } set { _Type = value; } }
        public string Value { get { return _Value; } set { _Value = value; } }
    }
    public class DataDiveObjective
    {
        private string _Type = "";
        private string _Value = "";

        public string Type { get { return _Type; } set { _Type = value; } }
        public string Value { get { return _Value; } set { _Value = value; } }
    }
    public class DataEvent
    {
        private string _Number = "";
        private string _Name = "";
        private string _Date = "2018";
        private string _Command = "";
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private List<DataEventItem> _Items = new List<DataEventItem>();

        public string Number { get { return _Number; } set { _Number = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Date { get { return _Date; } set { _Date = value; } }
        public string Command { get { return _Command; } set { _Command = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
        public List<DataEventItem> Items { get { return _Items; } set { _Items = value; } }
    }
    public class DataEventItem
    {
        private string _Name = "";
        private string _Type = "";

        public string Name { get { return _Name; } set { _Name = value; } }
        public string Type { get { return _Type; } set { _Type = value; } }
    }
}
