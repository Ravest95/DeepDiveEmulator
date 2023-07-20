namespace DeepDiveEmulator.Classes
{
    public class Settings
    {
        private SettingsServices _Services = new SettingsServices();
        private SettingsVersion _Version = new SettingsVersion();
        private SettingsMods _Mod = new SettingsMods();
        private SettingsDive _Dive = new SettingsDive();
        private SettingsEvent _Event = new SettingsEvent();
        private SettingsAssignment _Assignment = new SettingsAssignment();
        private SettingsCommon _Common = new SettingsCommon();

        public SettingsServices Services { get { return _Services; } set { _Services = value; } }
        public SettingsVersion Version { get { return _Version; } set { _Version = value; } }
        public SettingsMods Mod { get { return _Mod; } set { _Mod = value; } }
        public SettingsDive Dive { get { return _Dive; } set { _Dive = value; } }
        public SettingsEvent Event { get { return _Event; } set { _Event = value; } }
        public SettingsAssignment Assignment { get { return _Assignment; } set { _Assignment = value; } }
        public SettingsCommon Common { get { return _Common; } set { _Common = value; } }
    }
    public class SettingsServices
    {
        private string _IP = "127.0.0.1";
        private bool _ChangeRedirects = true;
        private bool _ChangeCertificates = true;
        private bool _StartServer = true;

        public string IP { get { return _IP; } set { _IP = value; } }
        public bool ChangeRedirects { get { return _ChangeRedirects; } set { _ChangeRedirects = value; } }
        public bool ChangeCertificates { get { return _ChangeCertificates; } set { _ChangeCertificates = value; } }
        public bool StartServer { get { return _StartServer; } set { _StartServer = value; } }
    }
    public class SettingsVersion
    {
        private string _Path = "";
        private string _PlayerId = "";
        private string _PlayerName = "Player";
        private string _Command = "-dx12 -nohmd";
        private int _SelectedId = -1;
        private string _SelectedNumber = "";
        private string _Search = "";

        public string Path { get { return _Path; } set { _Path = value; } }
        public string PlayerId { get { return _PlayerId; } set { _PlayerId = value; } }
        public string PlayerName { get { return _PlayerName; } set { _PlayerName = value; } }
        public string Command { get { return _Command; } set { _Command = value; } }
        public int SelectedId { get { return _SelectedId; } set { _SelectedId = value; } }
        public string SelectedNumber { get { return _SelectedNumber; } set { _SelectedNumber = value; } }
        public string Search { get { return _Search; } set { _Search = value; } }
    }
    public class SettingsMods
    {
        private int _SelectedId = -1;
        private string _SelectedNumber = "";
        private string _Search = "";

        public int SelectedId { get { return _SelectedId; } set { _SelectedId = value; } }
        public string SelectedNumber { get { return _SelectedNumber; } set { _SelectedNumber = value; } }
        public string Search { get { return _Search; } set { _Search = value; } }
    }
    public class SettingsDive
    {
        private uint _Seed = 0;
        private bool _LostDives = false;
        private int _SelectedId = -1;
        private string _SelectedNumber = "";
        private string _Search = "";

        public uint Seed { get { return _Seed; } set { _Seed = value; } }
        public bool LostDives { get { return _LostDives; } set { _LostDives = value; } }
        public int SelectedId { get { return _SelectedId; } set { _SelectedId = value; } }
        public string SelectedNumber { get { return _SelectedNumber; } set { _SelectedNumber = value; } }
        public string Search { get { return _Search; } set { _Search = value; } }
    }
    public class SettingsEvent
    {
        private string _Command = "";
        private bool _FreeBeers = false;
        private bool _LostEvents = false;
        private int _SelectedId = -1;
        private string _SelectedNumber = "";
        private string _Search = "";

        public string Command { get { return _Command; } set { _Command = value; } }
        public bool FreeBeers { get { return _FreeBeers; } set { _FreeBeers = value; } }
        public bool LostEvents { get { return _LostEvents; } set { _LostEvents = value; } }
        public int SelectedId { get { return _SelectedId; } set { _SelectedId = value; } }
        public string SelectedNumber { get { return _SelectedNumber; } set { _SelectedNumber = value; } }
        public string Search { get { return _Search; } set { _Search = value; } }
    }
    public class SettingsAssignment
    {
        private uint _Seed = 0;

        public uint Seed { get { return _Seed; } set { _Seed = value; } }
    }
    public class SettingsCommon
    {
        private double _PosX = 0;
        private double _PosY = 0;
        private double _SizeX = 1280;
        private double _SizeY = 800;

        public double PosX { get { return _PosX; } set { _PosX = value; } }
        public double PosY { get { return _PosY; } set { _PosY = value; } }
        public double SizeX { get { return _SizeX; } set { _SizeX = value; } }
        public double SizeY { get { return _SizeY; } set { _SizeY = value; } }
    }
}
