namespace DeepDiveEmulator.ClassDataSettings
{
    public class DataSettings
    {
        public DataSettingsServices Services = new DataSettingsServices();
        public DataSettingsGame Game = new DataSettingsGame();
        public DataSettingsDeepDive DeepDive = new DataSettingsDeepDive();
        public DataSettingsEvent Event = new DataSettingsEvent();
        public DataSettingsAssignment Assignment = new DataSettingsAssignment();
        public DataSettingsSettings Settings = new DataSettingsSettings();
    }
    public class DataSettingsServices
    {
        public string IP = "127.0.0.1";
        public bool ChangeRedirects = false;
        public bool StartServer = false;
    }
    public class DataSettingsGame
    {
        public string Path = "";
        public string PlayerId = "76561198085278322";
        public string PlayerName = "Player";
        public string Command = "-disablemodding -dx12 -nohmd";
        public string SelectedIdFake = "";
        public int SelectedIdReal = -1;
        public string Search = "";
    }
    public class DataSettingsDeepDive
    {
        public uint Seed = 0;
        public bool LostDeepDives = false;
        public string SelectedIdFake = "";
        public int SelectedIdReal = -1;
        public string Search = "";
    }
    public class DataSettingsEvent
    {
        public string Command = "";
        public bool FreeBeers = false;
        public bool LostEvents = false;
        public string SelectedIdFake = "";
        public int SelectedIdReal = -1;
        public string Search = "";
    }
    public class DataSettingsAssignment
    {
        public uint Seed = 0;
    }
    public class DataSettingsSettings
    {
        public double PositionX = 0;
        public double PositionY = 0;
        public double SizeX = 1280;
        public double SizeY = 800;
        public bool HideNotes = false;
    }
}
