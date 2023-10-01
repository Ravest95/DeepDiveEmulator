using DeepDiveEmulator.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using Clipboard = System.Windows.Clipboard;
using Color = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using DataFormats = System.Windows.DataFormats;
using DispatcherTimer = System.Windows.Threading.DispatcherTimer;
using Path = System.IO.Path;
using SymbolDisplay = Microsoft.CodeAnalysis.CSharp.SymbolDisplay;
using Window = System.Windows.Window;

namespace DeepDiveEmulator
{
    public partial class MainWindow : Window
    {
        #region TitleButtons
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void MaximizeClick(object sender, RoutedEventArgs e)
        {
            AdjustWindowSize();
        }
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    AdjustWindowSize();
                }
                else
                {
                    App.Current.MainWindow.DragMove();
                }
            }
        }
        private void AdjustWindowSize()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
                BtnMaximize.Content = "";
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
                BtnMaximize.Content = "";
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
        }
        #endregion

        public MainWindow()
        {
            {
                Process proc = Process.GetCurrentProcess();
                int count = Process.GetProcesses().Where(p => p.ProcessName == proc.ProcessName).Count();

                if (count > 1)
                {
                    App.Current.Shutdown();
                }
                InitializeComponent();
            }
        }

        // Variables.
        #region Variables
        // Path folders
        static string PathFoldAppDataLocal = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
        static string PathFoldPublic = Environment.ExpandEnvironmentVariables("%PUBLIC%");
        static string PathFoldWindows = Environment.ExpandEnvironmentVariables("%SystemRoot%");
        static string PathFoldApp = Application.StartupPath;
        static string PathFoldApache24 = Path.Combine(PathFoldApp, "Apache24");
        static string PathFoldApache24Certificates = Path.Combine(PathFoldApache24, "conf\\ssl.crt");
        static string PathFoldApache24URLs = Path.Combine(PathFoldApache24, "htdocs");
        static string PathFoldDataDives = Path.Combine(PathFoldApp, "Data\\Dives");
        static string PathFoldDataEvents = Path.Combine(PathFoldApp, "Data\\Events");
        static string PathFoldDataLanguages = Path.Combine(PathFoldApp, "Data\\Languages");
        static string PathFoldDataMods = Path.Combine(PathFoldApp, "Data\\Mods");
        static string PathFoldDataVersions = Path.Combine(PathFoldApp, "Data\\Versions");
        static string PathFoldGSE = Path.Combine(PathFoldApp, "GoldbergSteamEmulator");
        static string PathFoldModIO = Path.Combine(PathFoldPublic, "mod.io");
        static string PathFoldSavesVersions = Path.Combine(PathFoldApp, "Saves\\Versions");
        static string PathFoldSavesMods = Path.Combine(PathFoldApp, "Saves\\Mods");
        static string PathFoldSavesDives = Path.Combine(PathFoldApp, "Saves\\Dives");
        static string PathFoldWindowsETC = Path.Combine(PathFoldWindows, "System32\\drivers\\etc");
        // Path files.
        static string PathFileSettings = Path.Combine(PathFoldApp, "DeepDiveEmulator.ini");
        static string PathFileVersion = Path.Combine(PathFoldApp, "Version.txt");
        static string PathFileDataDiveParametersAnomalies = Path.Combine(PathFoldApp, "Data\\DiveParameters\\Anomalies.txt");
        static string PathFileDataDiveParametersMissions = Path.Combine(PathFoldApp, "Data\\DiveParameters\\Missions.txt");
        static string PathFileDataDiveParametersObjectives = Path.Combine(PathFoldApp, "Data\\DiveParameters\\Objectives.txt");
        static string PathFileDataDiveParametersRegions = Path.Combine(PathFoldApp, "Data\\DiveParameters\\Regions.txt");
        static string PathFileDataDiveParametersWarnings = Path.Combine(PathFoldApp, "Data\\DiveParameters\\Warnings.txt");
        static string PathFileDataURLsAssignments = Path.Combine(PathFoldApp, "Data\\URLs\\Assignments.txt");
        static string PathFileDataURLsDives = Path.Combine(PathFoldApp, "Data\\URLs\\Dives.txt");
        static string PathFileDataURLsEvents = Path.Combine(PathFoldApp, "Data\\URLs\\Events.txt");
        static string PathFileDataURLsFreeBeers = Path.Combine(PathFoldApp, "Data\\URLs\\FreeBeers.txt");
        static string PathFileDataVersionParameters = Path.Combine(PathFoldApp, "Data\\VersionParameters\\Branches.txt");
        static string PathFileGSEAccountName = Path.Combine(PathFoldGSE, "steam_settings\\settings\\account_name.txt");
        static string PathFileGSEUserSteamId = Path.Combine(PathFoldGSE, "steam_settings\\settings\\user_steam_id.txt");
        static string PathFileModIOGlobalSettings = Path.Combine(PathFoldAppDataLocal, "mod.io\\globalsettings.json");
        static string PathFileModIOState = Path.Combine(PathFoldPublic, "mod.io\\2475\\metadata\\state.json");
        static string PathFileModIOUser = Path.Combine(PathFoldAppDataLocal, "mod.io\\2475", WindowsIdentity.GetCurrent().User.ToString(), "user.json");
        static string PathFileWindowsHosts = Path.Combine(PathFoldWindows, "System32\\drivers\\etc\\hosts");
        //
        static string PathFileHTTPDEXE = Path.Combine(PathFoldApache24, "bin\\httpd.exe");
        static string PathFileHTTPDConfig = Path.Combine(PathFoldApache24, "conf\\httpd.conf");
        static string PathFileGSEColdClientLoader = Path.Combine(PathFoldGSE, "ColdClientLoader.ini");
        static string PathFileGSESteamClient_Loader = Path.Combine(PathFoldGSE, "steamclient_loader.exe");
        #endregion
        #region Data
        public static Data Data = new Data();
        public static Settings SettingsDefault = new Settings();
        public static Settings SettingsCurrent = new Settings();
        ObservableCollection<SrcVListVersion> SourceVlistVersions = new ObservableCollection<SrcVListVersion>();
        ObservableCollection<SrcVListMod> SourceVlistMods = new ObservableCollection<SrcVListMod>();
        ObservableCollection<SrcVListDive> SourceVlistDives = new ObservableCollection<SrcVListDive>();
        ObservableCollection<SrcVListEvent> SourceVlistEvents = new ObservableCollection<SrcVListEvent>();
        DispatcherTimer TmrSvcsStatus = new DispatcherTimer();
        DispatcherTimer TmrSvcsStatusRedirects = new DispatcherTimer();
        DispatcherTimer TmrSvcsStatusCertificates = new DispatcherTimer();
        DispatcherTimer TmrSvcsStatusServer = new DispatcherTimer();
        DispatcherTimer TmrVersSearch = new DispatcherTimer();
        DispatcherTimer TmrModsSearch = new DispatcherTimer();
        DispatcherTimer TmrDivsSearch = new DispatcherTimer();
        DispatcherTimer TmrEvtsSearch = new DispatcherTimer();
        DispatcherTimer TmrWindPos = new DispatcherTimer();
        DispatcherTimer TmrWindSize = new DispatcherTimer();
        #endregion

        // Functions.
        #region File App
        // DeepDiveEmulator.
        public void File_App_WrtKey_DDE_Services_IP(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("IP", "Services", inValue);

        }
        public void File_App_WrtKey_DDE_Services_ChangeRedirects(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("ChangeRedirects", "Services", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Services_ChangeCertificates(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("ChangeCertificates", "Services", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Services_StartServer(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("StartServer", "Services", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Version_Path(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Path", "Version", inValue);
        }
        public void File_App_WrtKey_DDE_Version_SelectedNumber(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("SelectedNumber", "Version", inValue);
        }
        public void File_App_WrtKey_DDE_Version_Branch(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Branch", "Version", inValue);
        }
        public void File_App_WrtKey_DDE_Dive_Seed(uint inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Seed", "Dive", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Dive_LostDives(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("LostDives", "Dive", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Dive_SelectedNumber(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("SelectedNumber", "Dive", inValue);
        }
        public void File_App_WrtKey_DDE_Event_Command(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Command", "Event", inValue);
        }
        public void File_App_WrtKey_DDE_Event_FreeBeers(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("FreeBeers", "Event", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Event_LostEvents(bool inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("LostEvents", "Event", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Event_SelectedNumber(string inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("SelectedNumber", "Event", inValue);
        }
        public void File_App_WrtKey_DDE_Assignment_Seed(uint inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Seed", "Assignment", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Common_PosX(double inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("PosX", "Common", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Common_PosY(double inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("PosY", "Common", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Common_SizeX(double inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("SizeX", "Common", inValue.ToString());
        }
        public void File_App_WrtKey_DDE_Common_SizeY(double inValue)
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("SizeY", "Common", inValue.ToString());
        }
        // Saves.
        public void File_App_WrtKey_SavesVersion_Path(int inIdVersion, string inValue)
        {
            string pathFile = Path.Combine(PathFoldSavesVersions, Data.Versions[inIdVersion].Number + ".ini");
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("Path", "SaveVersion", inValue);
        }
        public void File_App_DelFile_SavesVersion(int inIdVersion)
        {
            string pathFile = Path.Combine(PathFoldSavesVersions, Data.Versions[inIdVersion].Number + ".ini");
            File.Delete(pathFile);
        }
        #endregion
        #region File Apache24
        public void File_Apache24_Write_Assignments(uint inValue)
        {
            if (Data.URLs.Assignments.Count > 0)
            {
                string command = "{\"Seed\":" + inValue + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
                for (int i = 0; i < Data.URLs.Assignments.Count; i++)
                {
                    string pathFile = Path.Combine(PathFoldApache24URLs, Data.URLs.Assignments[i]);
                    Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                    File.WriteAllText(pathFile, command);
                }
            }
        }
        public void File_Apache24_Write_Dives(uint inValue)
        {
            if (Data.URLs.Dives.Count > 0)
            {
                string command = "{\"Seed\":" + inValue + ",\"SeedV2\":" + inValue + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
                for (int i = 0; i < Data.URLs.Dives.Count; i++)
                {
                    string pathFile = Path.Combine(PathFoldApache24URLs, Data.URLs.Dives[i]);
                    Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                    File.WriteAllText(pathFile, command);
                }
            }
        }
        public void File_Apache24_Write_Events(string inValue)
        {
            if (Data.URLs.Events.Count > 0)
            {
                // Remove spaces and replace "comma" with "quotes,comma,qoutes".
                string command = "{\"ActiveEvents\":[\"" + Regex.Replace(inValue, @" ", "").Replace(",", "\",\"") + "\"]}";
                for (int i = 0; i < Data.URLs.Events.Count; i++)
                {
                    string pathFile = Path.Combine(PathFoldApache24URLs, Data.URLs.Events[i]);
                    Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                    File.WriteAllText(pathFile, command);
                }
            }
        }
        public void File_Apache24_Write_FreeBeers(bool inValue)
        {
            if (Data.URLs.FreeBeers.Count > 0)
            {
                string command = "{\"FreeBeers\":" + inValue + "}";
                for (int i = 0; i < Data.URLs.FreeBeers.Count; i++)
                {
                    string pathFile = Path.Combine(PathFoldApache24URLs, Data.URLs.FreeBeers[i]);
                    Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                    File.WriteAllText(pathFile, command);
                }
            }
        }
        #endregion
        #region File Game
        public void File_Game_WrtKey_E_ENS_VerifyPeer(int inIdVersion, string inValue)
        {
            string pathFile;
            if (inIdVersion >= 0)
            {
                pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\Engine.ini"));
            }
            else
            {
                pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(SettingsCurrent.Version.Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\Engine.ini"));
            }
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("n.VerifyPeer", "/Script/Engine.NetworkSettings", inValue);
        }
        public void File_Game_WrtKey_GUS_SFSDUGC_CheckGameversion(int inIdVersion, string inValue)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                FileINI fileINI = new FileINI(pathFile);
                fileINI.WriteKey("CheckGameversion", "/Script/FSD.UserGeneratedContent", inValue);
            }
        }
        public void File_Game_WrtKey_GUS_SFSDUGC_CurrentBranchName(int inIdVersion, string inValue)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                FileINI fileINI = new FileINI(pathFile);
                fileINI.WriteKey("CurrentBranchName", "/Script/FSD.UserGeneratedContent", inValue);
            }
        }
        public void File_Game_WrtKey_GUS_SFSDUGC_ModsAreEnabled(int inIdVersion)
        {
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                FileINI fileINI = new FileINI(pathFile);
                for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                {
                    // No need to write disabled mods, because they will never load anyway.
                    if (Data.Versions[inIdVersion].Mods[i].IsEnabled == true)
                    {
                        fileINI.WriteKey(Data.Versions[inIdVersion].Mods[i].Number, "/Script/FSD.UserGeneratedContent", "True");
                    }
                }
            }
        }
        public void File_Game_DelSec_GUS_SFSDUGC(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                FileINI fileINI = new FileINI(pathFile);
                fileINI.DeleteSection("/Script/FSD.UserGeneratedContent");
            }
        }
        #endregion
        #region File GoldbergSteamEmulator
        public void File_GSE_WrtKey_CCL_SteamClient_AppId(string inValue)
        {
            string pathFile = PathFileGSEColdClientLoader;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("AppId", "SteamClient", inValue);
        }
        public void File_GSE_WrtKey_CCL_SteamClient_ExeCommandLine(string inValue)
        {
            string pathFile = PathFileGSEColdClientLoader;
            FileINI fileINI = new FileINI(pathFile);
            fileINI.WriteKey("ExeCommandLine", "SteamClient", inValue);
        }
        public void File_GSE_WrtFile_LocalSave()
        {
            string pathFile = Path.Combine(PathFoldGSE, "local_save.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            File.WriteAllText(pathFile, "steam_settings");
        }
        public void File_GSE_WrtFile_AccountName(string inValue)
        {
            string pathFile = PathFileGSEAccountName;
            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            File.WriteAllText(pathFile, inValue);
        }
        public void File_GSE_WrtFile_UserSteamId(string inValue)
        {
            string pathFile = PathFileGSEUserSteamId;
            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            File.WriteAllText(pathFile, inValue);
        }
        #endregion
        #region File ModIO
        public void File_ModIO_WrtFile_State(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                ModIOState data = new ModIOState();
                //
                if (Data.Versions[inIdVersion].Mods.Count > 0)
                {
                    for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                    {
                        if (Data.Versions[inIdVersion].Mods[i].IsEnabled == true)
                        {
                            ModIOStateMod mod = new ModIOStateMod();
                            string numberVersion = Data_Versions_Get_Number(inIdVersion);
                            string[] numberVersionParts = numberVersion.Split('.');
                            //
                            mod.ID = int.Parse(Data.Versions[inIdVersion].Mods[i].Number);
                            mod.PathOnDisk = Path.Combine(PathFoldDataMods, numberVersion, Data.Versions[inIdVersion].Mods[i].Number);
                            mod.Profile.tags[1].name = numberVersionParts[0] + "." + numberVersionParts[1];
                            //
                            data.Mods.Add(mod);
                        }
                    }
                }
                //
                string pathFile = PathFileModIOState;
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                File.WriteAllText(pathFile, JsonSerializer.Serialize(data));
            }
        }
        public void File_ModIO_WrtFile_GlobalSettings(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                ModIOGlobalSettings data = new ModIOGlobalSettings();
                //
                data.RootLocalStoragePath = PathFoldModIO;
                //
                string pathFile = PathFileModIOGlobalSettings;
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                File.WriteAllText(pathFile, JsonSerializer.Serialize(data));
            }
        }
        public void File_ModIO_WrtFile_User(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                ModIOUser data = new ModIOUser();
                //
                if (Data.Versions[inIdVersion].Mods.Count > 0)
                {
                    for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                    {
                        if (Data.Versions[inIdVersion].Mods[i].IsEnabled == true)
                        {
                            data.subscriptions.Add(int.Parse(Data.Versions[inIdVersion].Mods[i].Number));
                        }
                    }
                }
                //
                string pathFile = PathFileModIOUser;
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                File.WriteAllText(pathFile, JsonSerializer.Serialize(data));
            }
        }
        #endregion
        #region Version Load
        public void Version_Load()
        {
            string title = this.Title;
            string pathFile = PathFileVersion;
            if (File.Exists(pathFile) == true)
            {
                string[] lines = File.ReadAllLines(pathFile);
                if (lines.Length == 1)
                {
                    title = title + " - " + lines[0];
                }
            }
            LblTitle.Content = title;
        }
        #endregion
        #region Data Load
        public void Data_Load_All()
        {
            // Versions.
            Data_Versions_Load();
            // VersionParameters.
            Data_Load_VersionParameters_Branches();
            // Mods.
            Data_Mods_Load();
            // Dives.
            Data_Dives_Load();
            // DiveParameters.
            Data_Load_DiveParameters_Anomalies();
            Data_Load_DiveParameters_Missions();
            Data_Load_DiveParameters_Objectives();
            Data_Load_DiveParameters_Regions();
            Data_Load_DiveParameters_Warnings();
            // Events.
            Data_Events_Load();
            // URLs.
            Data_Load_URLs_Assignments();
            Data_Load_URLs_Dives();
            Data_Load_URLs_Events();
            Data_Load_URLs_FreeBeers();
        }
        // Versions.
        public void Data_Versions_Load()
        {
            // Clear the list, because function may be executed again.
            Data.Versions = new List<DataVersion>();
            // Create folder if it doesn't exist.
            Directory.CreateDirectory(PathFoldDataVersions);
            string[] pathFiles = Directory.GetFiles(PathFoldDataVersions, "*.ini", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                for (int i = 0; i < pathFiles.Length; i++)
                {
                    // Get file name without extention.
                    string number = Path.GetFileNameWithoutExtension(pathFiles[i]);
                    // Check if file name is correct.
                    if (String_Check_NumberVersion(number, out _) == true)
                    {
                        FileINI fileINI = new FileINI(pathFiles[i]);
                        // Create data.
                        DataVersion dataVersion = new DataVersion();
                        dataVersion.Number = number;
                        string? name = fileINI.ReadKeyString("Name", "Version");
                        if (name != null)
                        {
                            dataVersion.Name = name;
                        }
                        string? manifest = fileINI.ReadKeyString("Manifest", "Version");
                        if (manifest != null)
                        {
                            dataVersion.Manifest = manifest; // !!! Validate.
                        }
                        Byte? colorA = fileINI.ReadKeyByte("ColorA", "Version");
                        Byte? colorR = fileINI.ReadKeyByte("ColorR", "Version");
                        Byte? colorG = fileINI.ReadKeyByte("ColorG", "Version");
                        Byte? colorB = fileINI.ReadKeyByte("ColorB", "Version");
                        if (colorA != null && colorR != null && colorG != null && colorB != null)
                        {
                            dataVersion.Brush = new SolidColorBrush(Color.FromArgb(colorA.Value, colorR.Value, colorG.Value, colorB.Value));
                        }
                        // Add data to the list.
                        Data.Versions.Add(dataVersion);
                    }
                }
            }
        }
        public int Data_Versions_Get_Id(string inNumberVersion, List<DataVersion> inDataVersions = null)
        {
            int outIdVersion = -1;
            if (inNumberVersion != "")
            {
                List<DataVersion> dataVersions;
                if (inDataVersions != null)
                {
                    dataVersions = inDataVersions;
                }
                else
                {
                    dataVersions = Data.Versions;
                }
                if (dataVersions.Count > 0)
                {
                    for (int i = 0; i < dataVersions.Count; i++)
                    {
                        if (dataVersions[i].Number == inNumberVersion)
                        {
                            outIdVersion = i;
                            break;
                        }
                    }
                }
            }
            return outIdVersion;
        }
        public string Data_Versions_Get_Number(int inIdVersion)
        {
            string outNumberVersion = "";
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count)
            {
                outNumberVersion = Data.Versions[inIdVersion].Number;
            }
            return outNumberVersion;
        }
        public DataVersion Data_Versions_Get_Version(int inIdVersion)
        {
            DataVersion outDataVersion = new DataVersion();
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count)
            {
                outDataVersion = Data.Versions[inIdVersion];
            }
            return outDataVersion;
        }
        public List<DataVersion> Data_Versions_Get_Versions(string inText)
        {
            List<DataVersion> outDataVersions = new List<DataVersion>();
            if (Data.Versions.Count > 0)
            {
                for (int i = 0; i < Data.Versions.Count; i++)
                {
                    if (
                        Data.Versions[i].Number.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.Versions[i].Name.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                    )
                    {
                        outDataVersions.Add(Data.Versions[i]);
                    }
                }
            }
            return outDataVersions;
        }
        public List<DataVersion> Data_Versions_Sort_Versions(List<DataVersion> inDataVersions)
        {
            List<DataVersion> outDataVersions = inDataVersions.OrderByDescending(x => x.Number).ToList();
            return outDataVersions;
        }
        // VersionParameters.
        public void Data_Load_VersionParameters_Branches()
        {
            // Clear the list, because it may be reloaded later.
            Data.VersionParameters.Branches = new List<string>();
            // Add epmty string.
            Data.VersionParameters.Branches.Add("");
            // Load from file.
            if (File.Exists(PathFileDataVersionParameters) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataVersionParameters);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.VersionParameters.Branches.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataVersionParameters));
                File.Create(PathFileDataVersionParameters);
            }
        }
        // Mods.
        public void Data_Mods_Load()
        {
            if (Data.Versions.Count > 0)
            {
                // Clear the lists, because function may be executed again.
                for (int i = 0; i < Data.Versions.Count; i++)
                {
                    Data.Versions[i].Mods = new List<DataMod>();
                }
                // Create folder if it doesn't exist.
                Directory.CreateDirectory(PathFoldDataMods);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldDataMods, "*", SearchOption.TopDirectoryOnly);
                if (pathFoldsVersion.Length > 0)
                {
                    for (int iVersion = 0; iVersion < pathFoldsVersion.Length; iVersion++)
                    {
                        string numberVersion = Path.GetFileName(pathFoldsVersion[iVersion]);
                        // Check if version exists.
                        int idVersion = Data_Versions_Get_Id(numberVersion);
                        if (idVersion >= 0)
                        {
                            string[] pathFoldsMod = Directory.GetDirectories(pathFoldsVersion[iVersion], "*", SearchOption.TopDirectoryOnly);
                            if (pathFoldsMod.Length > 0)
                            {
                                for (int iMod = 0; iMod < pathFoldsMod.Length; iMod++)
                                {
                                    string numberMod = Path.GetFileName(pathFoldsMod[iMod]);
                                    // Check if number is correct.
                                    if (String_Check_NumberMod(numberMod) == true)
                                    {
                                        // Check if file "File.pak" exists, because it is essential for the mod.
                                        if (File.Exists(Path.Combine(pathFoldsMod[iMod], "File.pak")) == true)
                                        {
                                            DataMod dataMod = new DataMod();
                                            dataMod.Number = numberMod;
                                            // No need to check if file "Info.ini" exists.
                                            string pathFileInfo = Path.Combine(pathFoldsMod[iMod], "Info.ini");
                                            FileINI fileInfo = new FileINI(pathFileInfo);

                                            string? name = fileInfo.ReadKeyString("Name", "Mod");
                                            if (name != null)
                                            {
                                                dataMod.Name = name;
                                            }
                                            int? time = fileInfo.ReadKeyInt("Time", "Mod");
                                            if (time != null)
                                            {
                                                dataMod.Time = time.Value;
                                            }
                                            string? description = fileInfo.ReadKeyString("Description", "Mod");
                                            if (description != null)
                                            {
                                                dataMod.Description = description;
                                            }
                                            dataMod.BrushBack = Data.Versions[idVersion].Brush;
                                            // Add mod.
                                            Data.Versions[idVersion].Mods.Add(dataMod);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public int Data_Mods_Get_Id(int inIdVersion, string inNumberMod, List<DataMod> inDataMods = null)
        {
            int outIdMod = -1;
            if (inIdVersion >= 0 && inNumberMod != "")
            {
                List<DataMod> dataMods;
                if (inDataMods != null)
                {
                    dataMods = inDataMods;
                }
                else
                {
                    dataMods = Data.Versions[inIdVersion].Mods;
                }
                if (dataMods.Count > 0)
                {
                    for (int i = 0; i < dataMods.Count; i++)
                    {
                        if (dataMods[i].Number == inNumberMod)
                        {
                            outIdMod = i;
                            break;
                        }
                    }
                }
            }
            return outIdMod;
        }
        public string Data_Mods_Get_Number(int inIdVersion, int inIdMod)
        {
            string outNumberMod = "";
            // Index can be bigger than range, when version was deleted and Data was reloaded.
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdMod >= 0 && inIdMod < Data.Versions[inIdVersion].Mods.Count)
            {
                outNumberMod = Data.Versions[inIdVersion].Mods[inIdMod].Number;
            }
            return outNumberMod;
        }
        public DataMod Data_Mods_Get_Mod(int inIdVersion, int inIdMod)
        {
            DataMod outDataMod = new DataMod();
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdMod >= 0 && inIdMod < Data.Versions[inIdVersion].Mods.Count)
            {
                outDataMod = Data.Versions[inIdVersion].Mods[inIdMod];
            }
            return outDataMod;
        }
        public List<DataMod> Data_Mods_Get_Mods(int inIdVersion, string inText)
        {
            List<DataMod> outDataMods = new List<DataMod>();
            if (inIdVersion >= 0 && inIdVersion <= Data.Versions.Count - 1 && Data.Versions[inIdVersion].Mods.Count > 0)
            {
                for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                {
                    if (
                        Data.Versions[inIdVersion].Mods[i].Number.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.Versions[inIdVersion].Mods[i].Name.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                    )
                    {
                        outDataMods.Add(Data.Versions[inIdVersion].Mods[i]);
                    }
                }
            }
            return outDataMods;
        }
        public List<DataMod> Data_Mods_Sort_Mods(List<DataMod> inDataMods)
        {
            List<DataMod> outDataMods = inDataMods.OrderByDescending(x => int.Parse(x.Number)).ToList();
            return outDataMods;
        }
        // DiveParameters.
        public void Data_Load_DiveParameters_Anomalies()
        {
            // Clear the list, because it may be reloaded later.
            Data.DiveParameters.Anomalies = new List<string>();
            // Add epmty string.
            Data.DiveParameters.Anomalies.Add("");
            // Load from file.
            if (File.Exists(PathFileDataDiveParametersAnomalies) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataDiveParametersAnomalies);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.DiveParameters.Anomalies.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataDiveParametersAnomalies));
                File.Create(PathFileDataDiveParametersAnomalies);
            }
        }
        public void Data_Load_DiveParameters_Missions()
        {
            // Clear the list, because it may be reloaded later.
            Data.DiveParameters.Missions = new List<string>();
            // Add epmty string.
            Data.DiveParameters.Missions.Add("");
            // Load from file.
            if (File.Exists(PathFileDataDiveParametersMissions) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataDiveParametersMissions);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.DiveParameters.Missions.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataDiveParametersMissions));
                File.Create(PathFileDataDiveParametersMissions);
            }
        }
        public void Data_Load_DiveParameters_Objectives()
        {
            // Clear the list, because it may be reloaded later.
            Data.DiveParameters.Objectives = new List<string>();
            // Add epmty string.
            Data.DiveParameters.Objectives.Add("");
            // Load from file.
            if (File.Exists(PathFileDataDiveParametersObjectives) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataDiveParametersObjectives);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.DiveParameters.Objectives.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataDiveParametersObjectives));
                File.Create(PathFileDataDiveParametersObjectives);
            }
        }
        public void Data_Load_DiveParameters_Regions()
        {
            // Clear the list, because it may be reloaded later.
            Data.DiveParameters.Regions = new List<string>();
            // Add epmty string.
            Data.DiveParameters.Regions.Add("");
            // Load from file.
            if (File.Exists(PathFileDataDiveParametersRegions) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataDiveParametersRegions);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.DiveParameters.Regions.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataDiveParametersRegions));
                File.Create(PathFileDataDiveParametersRegions);
            }
        }
        public void Data_Load_DiveParameters_Warnings()
        {
            // Clear the list, because it may be reloaded later.
            Data.DiveParameters.Warnings = new List<string>();
            // Add epmty string.
            Data.DiveParameters.Warnings.Add("");
            // Load from file.
            if (File.Exists(PathFileDataDiveParametersWarnings) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataDiveParametersWarnings);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.DiveParameters.Warnings.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataDiveParametersWarnings));
                File.Create(PathFileDataDiveParametersWarnings);
            }
        }
        // Dives.
        public void Data_Dives_Load()
        {
            if (Data.Versions.Count > 0)
            {
                // Clear the lists, because function may be executed again.
                for (int i = 0; i < Data.Versions.Count; i++)
                {
                    Data.Versions[i].Dives = new List<DataDive>();
                }
                // Create folder if it doesn't exist.
                Directory.CreateDirectory(PathFoldDataDives);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldDataDives, "*", SearchOption.TopDirectoryOnly);
                if (pathFoldsVersion.Length > 0)
                {
                    for (int iVersion = 0; iVersion < pathFoldsVersion.Length; iVersion++)
                    {
                        string numberVersion = Path.GetFileName(pathFoldsVersion[iVersion]);
                        // Check if version exists.
                        int idVersion = Data_Versions_Get_Id(numberVersion);
                        if (idVersion >= 0)
                        {
                            string[] pathFilesDive = Directory.GetFiles(pathFoldsVersion[iVersion], "*.ini", SearchOption.TopDirectoryOnly);
                            if (pathFilesDive.Length > 0)
                            {
                                for (int iDive = 0; iDive < pathFilesDive.Length; iDive++)
                                {
                                    string numberDive = Path.GetFileNameWithoutExtension(pathFilesDive[iDive]);
                                    // Check if number is correct.
                                    if (String_Check_NumberDive(numberDive) == true)
                                    {
                                        FileINI fileINI = new FileINI(pathFilesDive[iDive]);
                                        // Define dive.
                                        DataDive dataDive = new DataDive();
                                        dataDive.Number = numberDive;
                                        uint? seed = fileINI.ReadKeyUInt("Seed", "Dive");
                                        if (seed != null)
                                        {
                                            dataDive.Seed = seed.Value;
                                        }
                                        dataDive.BrushBack = Data.Versions[idVersion].Brush;
                                        string? eventNumber = fileINI.ReadKeyString("Event", "Dive");
                                        if (eventNumber != null)
                                        {
                                            dataDive.EventNumber = eventNumber;
                                        }
                                        string? date = fileINI.ReadKeyString("Date", "Dive");
                                        if (date != null)
                                        {
                                            dataDive.Date = date;
                                        }
                                        string? normalName = fileINI.ReadKeyString("NormalName", "Dive");
                                        if (normalName != null)
                                        {
                                            dataDive.Normal.Name = normalName;
                                        }
                                        string? normalRegion = fileINI.ReadKeyString("NormalRegion", "Dive");
                                        if (normalRegion != null)
                                        {
                                            dataDive.Normal.Region = normalRegion;
                                        }
                                        string? normalMissionType1 = fileINI.ReadKeyString("NormalMissionType1", "Dive");
                                        if (normalMissionType1 != null)
                                        {
                                            dataDive.Normal.Missions[0].Type = normalMissionType1;
                                        }
                                        string? normalMissionType2 = fileINI.ReadKeyString("NormalMissionType2", "Dive");
                                        if (normalMissionType2 != null)
                                        {
                                            dataDive.Normal.Missions[1].Type = normalMissionType2;
                                        }
                                        string? normalMissionType3 = fileINI.ReadKeyString("NormalMissionType3", "Dive");
                                        if (normalMissionType3 != null)
                                        {
                                            dataDive.Normal.Missions[2].Type = normalMissionType3;
                                        }
                                        string? normalMissionValue1 = fileINI.ReadKeyString("NormalMissionValue1", "Dive");
                                        if (normalMissionValue1 != null)
                                        {
                                            dataDive.Normal.Missions[0].Value = normalMissionValue1;
                                        }
                                        string? normalMissionValue2 = fileINI.ReadKeyString("NormalMissionValue2", "Dive");
                                        if (normalMissionValue2 != null)
                                        {
                                            dataDive.Normal.Missions[1].Value = normalMissionValue2;
                                        }
                                        string? normalMissionValue3 = fileINI.ReadKeyString("NormalMissionValue3", "Dive");
                                        if (normalMissionValue3 != null)
                                        {
                                            dataDive.Normal.Missions[2].Value = normalMissionValue3;
                                        }
                                        string? normalObjectiveType1 = fileINI.ReadKeyString("NormalObjectiveType1", "Dive");
                                        if (normalObjectiveType1 != null)
                                        {
                                            dataDive.Normal.Objectives[0].Type = normalObjectiveType1;
                                        }
                                        string? normalObjectiveType2 = fileINI.ReadKeyString("NormalObjectiveType2", "Dive");
                                        if (normalObjectiveType2 != null)
                                        {
                                            dataDive.Normal.Objectives[1].Type = normalObjectiveType2;
                                        }
                                        string? normalObjectiveType3 = fileINI.ReadKeyString("NormalObjectiveType3", "Dive");
                                        if (normalObjectiveType3 != null)
                                        {
                                            dataDive.Normal.Objectives[2].Type = normalObjectiveType3;
                                        }
                                        string? normalObjectiveValue1 = fileINI.ReadKeyString("NormalObjectiveValue1", "Dive");
                                        if (normalObjectiveValue1 != null)
                                        {
                                            dataDive.Normal.Objectives[0].Value = normalObjectiveValue1;
                                        }
                                        string? normalObjectiveValue2 = fileINI.ReadKeyString("NormalObjectiveValue2", "Dive");
                                        if (normalObjectiveValue2 != null)
                                        {
                                            dataDive.Normal.Objectives[1].Value = normalObjectiveValue2;
                                        }
                                        string? normalObjectiveValue3 = fileINI.ReadKeyString("NormalObjectiveValue3", "Dive");
                                        if (normalObjectiveValue3 != null)
                                        {
                                            dataDive.Normal.Objectives[2].Value = normalObjectiveValue3;
                                        }
                                        string? normalWarning1 = fileINI.ReadKeyString("NormalWarning1", "Dive");
                                        if (normalWarning1 != null)
                                        {
                                            dataDive.Normal.Warnings[0] = normalWarning1;
                                        }
                                        string? normalWarning2 = fileINI.ReadKeyString("NormalWarning2", "Dive");
                                        if (normalWarning2 != null)
                                        {
                                            dataDive.Normal.Warnings[1] = normalWarning2;
                                        }
                                        string? normalWarning3 = fileINI.ReadKeyString("NormalWarning3", "Dive");
                                        if (normalWarning3 != null)
                                        {
                                            dataDive.Normal.Warnings[2] = normalWarning3;
                                        }
                                        string? normalAnomaly1 = fileINI.ReadKeyString("NormalAnomaly1", "Dive");
                                        if (normalAnomaly1 != null)
                                        {
                                            dataDive.Normal.Anomalies[0] = normalAnomaly1;
                                        }
                                        string? normalAnomaly2 = fileINI.ReadKeyString("NormalAnomaly2", "Dive");
                                        if (normalAnomaly2 != null)
                                        {
                                            dataDive.Normal.Anomalies[1] = normalAnomaly2;
                                        }
                                        string? normalAnomaly3 = fileINI.ReadKeyString("NormalAnomaly3", "Dive");
                                        if (normalAnomaly3 != null)
                                        {
                                            dataDive.Normal.Anomalies[2] = normalAnomaly3;
                                        }
                                        string? eliteName = fileINI.ReadKeyString("EliteName", "Dive");
                                        if (eliteName != null)
                                        {
                                            dataDive.Elite.Name = eliteName;
                                        }
                                        string? eliteRegion = fileINI.ReadKeyString("EliteRegion", "Dive");
                                        if (eliteRegion != null)
                                        {
                                            dataDive.Elite.Region = eliteRegion;
                                        }
                                        string? eliteMissionType1 = fileINI.ReadKeyString("EliteMissionType1", "Dive");
                                        if (eliteMissionType1 != null)
                                        {
                                            dataDive.Elite.Missions[0].Type = eliteMissionType1;
                                        }
                                        string? eliteMissionType2 = fileINI.ReadKeyString("EliteMissionType2", "Dive");
                                        if (eliteMissionType2 != null)
                                        {
                                            dataDive.Elite.Missions[1].Type = eliteMissionType2;
                                        }
                                        string? eliteMissionType3 = fileINI.ReadKeyString("EliteMissionType3", "Dive");
                                        if (eliteMissionType3 != null)
                                        {
                                            dataDive.Elite.Missions[2].Type = eliteMissionType3;
                                        }
                                        string? eliteMissionValue1 = fileINI.ReadKeyString("EliteMissionValue1", "Dive");
                                        if (eliteMissionValue1 != null)
                                        {
                                            dataDive.Elite.Missions[0].Value = eliteMissionValue1;
                                        }
                                        string? eliteMissionValue2 = fileINI.ReadKeyString("EliteMissionValue2", "Dive");
                                        if (eliteMissionValue2 != null)
                                        {
                                            dataDive.Elite.Missions[1].Value = eliteMissionValue2;
                                        }
                                        string? eliteMissionValue3 = fileINI.ReadKeyString("EliteMissionValue3", "Dive");
                                        if (eliteMissionValue3 != null)
                                        {
                                            dataDive.Elite.Missions[2].Value = eliteMissionValue3;
                                        }
                                        string? eliteObjectiveType1 = fileINI.ReadKeyString("EliteObjectiveType1", "Dive");
                                        if (eliteObjectiveType1 != null)
                                        {
                                            dataDive.Elite.Objectives[0].Type = eliteObjectiveType1;
                                        }
                                        string? eliteObjectiveType2 = fileINI.ReadKeyString("EliteObjectiveType2", "Dive");
                                        if (eliteObjectiveType2 != null)
                                        {
                                            dataDive.Elite.Objectives[1].Type = eliteObjectiveType2;
                                        }
                                        string? eliteObjectiveType3 = fileINI.ReadKeyString("EliteObjectiveType3", "Dive");
                                        if (eliteObjectiveType3 != null)
                                        {
                                            dataDive.Elite.Objectives[2].Type = eliteObjectiveType3;
                                        }
                                        string? eliteObjectiveValue1 = fileINI.ReadKeyString("EliteObjectiveValue1", "Dive");
                                        if (eliteObjectiveValue1 != null)
                                        {
                                            dataDive.Elite.Objectives[0].Value = eliteObjectiveValue1;
                                        }
                                        string? eliteObjectiveValue2 = fileINI.ReadKeyString("EliteObjectiveValue2", "Dive");
                                        if (eliteObjectiveValue2 != null)
                                        {
                                            dataDive.Elite.Objectives[1].Value = eliteObjectiveValue2;
                                        }
                                        string? eliteObjectiveValue3 = fileINI.ReadKeyString("EliteObjectiveValue3", "Dive");
                                        if (eliteObjectiveValue3 != null)
                                        {
                                            dataDive.Elite.Objectives[2].Value = eliteObjectiveValue3;
                                        }
                                        string? eliteWarning1 = fileINI.ReadKeyString("EliteWarning1", "Dive");
                                        if (eliteWarning1 != null)
                                        {
                                            dataDive.Elite.Warnings[0] = eliteWarning1;
                                        }
                                        string? eliteWarning2 = fileINI.ReadKeyString("EliteWarning2", "Dive");
                                        if (eliteWarning2 != null)
                                        {
                                            dataDive.Elite.Warnings[1] = eliteWarning2;
                                        }
                                        string? eliteWarning3 = fileINI.ReadKeyString("EliteWarning3", "Dive");
                                        if (eliteWarning3 != null)
                                        {
                                            dataDive.Elite.Warnings[2] = eliteWarning3;
                                        }
                                        string? eliteAnomaly1 = fileINI.ReadKeyString("EliteAnomaly1", "Dive");
                                        if (eliteAnomaly1 != null)
                                        {
                                            dataDive.Elite.Anomalies[0] = eliteAnomaly1;
                                        }
                                        string? eliteAnomaly2 = fileINI.ReadKeyString("EliteAnomaly2", "Dive");
                                        if (eliteAnomaly2 != null)
                                        {
                                            dataDive.Elite.Anomalies[1] = eliteAnomaly2;
                                        }
                                        string? eliteAnomaly3 = fileINI.ReadKeyString("EliteAnomaly3", "Dive");
                                        if (eliteAnomaly3 != null)
                                        {
                                            dataDive.Elite.Anomalies[2] = eliteAnomaly3;
                                        }
                                        // Add dive to the list.
                                        Data.Versions[idVersion].Dives.Add(dataDive);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public int Data_Dives_Get_Id(int inIdVersion, string inNumberDive, List<DataDive> inDataDives = null)
        {
            int outIdDive = -1;
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inNumberDive != "")
            {
                List<DataDive> dataDives;
                if (inDataDives != null)
                {
                    dataDives = inDataDives;
                }
                else
                {
                    dataDives = Data.Versions[inIdVersion].Dives;
                }
                if (dataDives.Count > 0)
                {
                    for (int i = 0; i < dataDives.Count; i++)
                    {
                        if (dataDives[i].Number == inNumberDive)
                        {
                            outIdDive = i;
                            break;
                        }
                    }
                }
            }
            return outIdDive;
        }
        public string Data_Dives_Get_Number(int inIdVersion, int inIdDive)
        {
            string outNumberDive = "";
            // Index can be bigger than range, when version was deleted and Data was reloaded.
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdDive >= 0 && inIdDive < Data.Versions[inIdVersion].Dives.Count)
            {
                outNumberDive = Data.Versions[inIdVersion].Dives[inIdDive].Number;
            }
            return outNumberDive;
        }
        public DataDive Data_Dives_Get_Dive(int inIdVersion, int inIdDive)
        {
            DataDive outDataDive = new DataDive();
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdDive >= 0 && inIdDive < Data.Versions[inIdVersion].Dives.Count)
            {
                outDataDive = Data.Versions[inIdVersion].Dives[inIdDive];
            }
            return outDataDive;
        }
        public List<DataDive> Data_Dives_Get_Dives(int inIdVersion, string inText)
        {
            List<DataDive> outDataDives = new List<DataDive>();
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && Data.Versions[inIdVersion].Dives.Count > 0)
            {
                for (int i = 0; i < Data.Versions[inIdVersion].Dives.Count; i++)
                {
                    if (Data.Versions[inIdVersion].Dives[i].Seed != 0 || Data.Versions[inIdVersion].Dives[i].Seed == 0 && SettingsCurrent.Dive.LostDives == true)
                    {
                        if (
                            Data.Versions[inIdVersion].Dives[i].Number.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Seed.ToString().Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Name.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Name.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Missions[0].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Missions[1].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Missions[2].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Missions[0].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Missions[1].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Missions[2].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Objectives[0].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Objectives[1].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Objectives[2].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Objectives[0].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Objectives[1].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Objectives[2].Type.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Warnings[0].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Warnings[1].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Warnings[2].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Warnings[0].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Warnings[1].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Warnings[2].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Anomalies[0].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Anomalies[1].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Normal.Anomalies[2].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Anomalies[0].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Anomalies[1].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Dives[i].Elite.Anomalies[2].Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                        )
                        {
                            outDataDives.Add(Data.Versions[inIdVersion].Dives[i]);
                        }
                    }
                }
            }
            return outDataDives;
        }
        public List<DataDive> Data_Dives_Sort_Dives(List<DataDive> inDataDives)
        {
            List<DataDive> outDataDives = inDataDives.OrderByDescending(x => int.Parse(x.Number)).ToList();
            return outDataDives;
        }
        // Events.
        public void Data_Events_Load()
        {
            if (Data.Versions.Count > 0)
            {
                // Clear the lists, because function may be executed again.
                for (int i = 0; i < Data.Versions.Count; i++)
                {
                    Data.Versions[i].Events = new List<DataEvent>();
                }
                // Create folder if it doesn't exist.
                Directory.CreateDirectory(PathFoldDataEvents);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldDataEvents, "*", SearchOption.TopDirectoryOnly);
                if (pathFoldsVersion.Length > 0)
                {
                    for (int iVersion = 0; iVersion < pathFoldsVersion.Length; iVersion++)
                    {
                        string numberVersion = Path.GetFileName(pathFoldsVersion[iVersion]);
                        // Check if version exists.
                        int idVersion = Data_Versions_Get_Id(numberVersion);
                        if (idVersion >= 0)
                        {
                            string[] pathFilesEvent = Directory.GetFiles(pathFoldsVersion[iVersion], "*.ini", SearchOption.TopDirectoryOnly);
                            if (pathFilesEvent.Length > 0)
                            {
                                for (int iEvent = 0; iEvent < pathFilesEvent.Length; iEvent++)
                                {
                                    string numberEvent = Path.GetFileNameWithoutExtension(pathFilesEvent[iEvent]);
                                    // Check if number is correct.
                                    if (String_Check_NumberEvent(numberEvent) == true)
                                    {
                                        FileINI fileINI = new FileINI(pathFilesEvent[iEvent]);
                                        // Define event.
                                        DataEvent dataEvent = new DataEvent();
                                        dataEvent.Number = numberEvent;
                                        string? name = fileINI.ReadKeyString("Name", "Event");
                                        if (name != null)
                                        {
                                            dataEvent.Name = name;
                                        }
                                        string? date = fileINI.ReadKeyString("Date", "Event");
                                        if (date != null)
                                        {
                                            dataEvent.Date = date; // Validate Date.
                                        }
                                        string? command = fileINI.ReadKeyString("Command", "Event");
                                        if (command != null)
                                        {
                                            dataEvent.Command = command;
                                        }
                                        dataEvent.BrushBack = Data.Versions[idVersion].Brush;
                                        bool check = true;
                                        int count = 1;
                                        while (check == true)
                                        {
                                            string? itemName = fileINI.ReadKeyString("ItemName", "Event");
                                            string? itemType = fileINI.ReadKeyString("ItemType", "Event");
                                            if (itemName != null && itemType != null)
                                            {
                                                DataEventItem item = new DataEventItem();
                                                item.Name = itemName;
                                                item.Type = itemType;
                                                //
                                                dataEvent.Items.Add(item);
                                                //
                                                count = count + 1;
                                            }
                                            else
                                            {
                                                // Turn loop off.
                                                check = false;
                                            }

                                        }
                                        // Add event to the list.
                                        Data.Versions[idVersion].Events.Add(dataEvent);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public int Data_Events_Get_Id(int inIdVersion, string inNumberEvent, List<DataEvent> inDataEvents = null)
        {
            int outIdEvent = -1;
            if (inIdVersion >= 0 && inNumberEvent != "")
            {
                List<DataEvent> dataEvents;
                if (inDataEvents != null)
                {
                    dataEvents = inDataEvents;
                }
                else
                {
                    dataEvents = Data.Versions[inIdVersion].Events;
                }
                if (dataEvents.Count > 0)
                {
                    for (int i = 0; i < dataEvents.Count; i++)
                    {
                        if (dataEvents[i].Number == inNumberEvent)
                        {
                            outIdEvent = i;
                            break;
                        }
                    }
                }
            }
            return outIdEvent;
        }
        public string Data_Events_Get_Number(int inIdVersion, int inIdEvent)
        {
            string outNumberEvent = "";
            // Index can be bigger than range, when version was deleted and Data was reloaded.
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdEvent >= 0 && inIdEvent < Data.Versions[inIdVersion].Events.Count)
            {
                outNumberEvent = Data.Versions[inIdVersion].Events[inIdEvent].Number;
            }
            return outNumberEvent;
        }
        public DataEvent Data_Events_Get_Event(int inIdVersion, int inIdEvent)
        {
            DataEvent outDataEvent = new DataEvent();
            if (inIdVersion >= 0 && inIdVersion < Data.Versions.Count && inIdEvent >= 0 && inIdEvent < Data.Versions[inIdVersion].Events.Count)
            {
                outDataEvent = Data.Versions[inIdVersion].Events[inIdEvent];
            }
            return outDataEvent;
        }
        public List<DataEvent> Data_Events_Get_Events(int inIdVersion, string inText)
        {
            List<DataEvent> outDataEvents = new List<DataEvent>();
            if (inIdVersion >= 0 && inIdVersion <= Data.Versions.Count - 1 && Data.Versions[inIdVersion].Events.Count > 0)
            {
                for (int i = 0; i < Data.Versions[inIdVersion].Events.Count; i++)
                {
                    if (Data.Versions[inIdVersion].Events[i].Command != "" || Data.Versions[inIdVersion].Events[i].Command == "" && SettingsCurrent.Event.LostEvents == true)
                    {
                        if (
                            Data.Versions[inIdVersion].Events[i].Number.ToString().Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Events[i].Name.Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                            ||
                            Data.Versions[inIdVersion].Events[i].Date.ToString().Contains(inText, StringComparison.CurrentCultureIgnoreCase) == true
                        )
                        {
                            outDataEvents.Add(Data.Versions[inIdVersion].Events[i]);
                        }
                    }
                }
            }
            return outDataEvents;
        }
        public List<DataEvent> Data_Events_Sort_Events(List<DataEvent> inDataEvents)
        {
            List<DataEvent> outDataEvents = inDataEvents.OrderByDescending(x => int.Parse(x.Number)).ToList();
            return outDataEvents;
        }
        // URLs.
        public void Data_Load_URLs_Assignments()
        {
            // Clear the list, because it may be reloaded later.
            Data.URLs.Assignments = new List<string>();
            // Load from file.
            if (File.Exists(PathFileDataURLsAssignments) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataURLsAssignments);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.URLs.Assignments.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataURLsAssignments));
                File.Create(PathFileDataURLsAssignments);
            }
        }
        public void Data_Load_URLs_Dives()
        {
            // Clear the list, because it may be reloaded later.
            Data.URLs.Dives = new List<string>();
            // Load from file.
            if (File.Exists(PathFileDataURLsDives) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataURLsDives);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.URLs.Dives.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataURLsDives));
                File.Create(PathFileDataURLsDives);
            }
        }
        public void Data_Load_URLs_Events()
        {
            // Clear the list, because it may be reloaded later.
            Data.URLs.Events = new List<string>();
            // Load from file.
            if (File.Exists(PathFileDataURLsEvents) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataURLsEvents);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.URLs.Events.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataURLsEvents));
                File.Create(PathFileDataURLsEvents);
            }
        }
        public void Data_Load_URLs_FreeBeers()
        {
            // Clear the list, because it may be reloaded later.
            Data.URLs.FreeBeers = new List<string>();
            // Load from file.
            if (File.Exists(PathFileDataURLsFreeBeers) == true)
            {
                string[] lines = File.ReadAllLines(PathFileDataURLsFreeBeers);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Data.URLs.FreeBeers.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileDataURLsFreeBeers));
                File.Create(PathFileDataURLsFreeBeers);
            }
        }
        #endregion
        #region Settings Save
        // Services.
        public void Settings_Save_Services_IP(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Services.IP = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Services_IP(inValue);
        }
        public void Settings_Save_Services_ChangeRedirects(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Services.ChangeRedirects = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Services_ChangeRedirects(inValue);
        }
        public void Settings_Save_Services_ChangeCertificates(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Services.ChangeCertificates = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Services_ChangeCertificates(inValue);
        }
        public void Settings_Save_Services_StartServer(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Services.StartServer = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Services_StartServer(inValue);
        }
        // Version.
        public void Settings_Save_Version_Path(int inIdVersion, string inValue)
        {
            if (inIdVersion >= 0)
            {
                // Internal Setting.
                Data.Versions[inIdVersion].Path = inValue;
                // External Setting.
                if (inValue != "")
                {
                    File_App_WrtKey_SavesVersion_Path(inIdVersion, inValue);
                }
                else
                {
                    File_App_DelFile_SavesVersion(inIdVersion);
                }
            }
            else
            {
                // Internal Setting.
                SettingsCurrent.Version.Path = inValue;
                // External Setting.
                File_App_WrtKey_DDE_Version_Path(inValue);
            }
        }
        public void Settings_Save_Version_Branch(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Version.Branch = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Version_Branch(inValue);
        }
        public void Settings_Save_Version_PlayerId(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Version.PlayerId = inValue;
            // External Setting.
            File_GSE_WrtFile_UserSteamId(inValue);
        }
        public void Settings_Save_Version_PlayerName(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Version.PlayerName = inValue;
            // External Setting.
            File_GSE_WrtFile_AccountName(inValue);
        }
        public void Settings_Save_Version_Command(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Version.Command = inValue;
            // External Setting.
            File_GSE_WrtKey_CCL_SteamClient_ExeCommandLine(inValue);
        }
        public void Settings_Save_Version_Search(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Version.Search = inValue;
        }
        public void Settings_Save_Version_SelectedId(int inIdVersion)
        {
            // Internal Setting.
            SettingsCurrent.Version.SelectedId = inIdVersion;
            string numberVersion = Data_Versions_Get_Number(inIdVersion);
            SettingsCurrent.Version.SelectedNumber = numberVersion;
            // External Setting.
            File_App_WrtKey_DDE_Version_SelectedNumber(numberVersion);
        }
        // Mod.
        public void Settings_Save_Mod_Search(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Mod.Search = inValue;
        }
        public void Settings_Save_Mod_SelectedId(int inIdMod)
        {
            // Internal Setting.
            SettingsCurrent.Mod.SelectedId = inIdMod;
            string numberMod = Data_Mods_Get_Number(SettingsCurrent.Version.SelectedId, inIdMod);
            SettingsCurrent.Mod.SelectedNumber = numberMod;
        }
        public void Settings_Save_Mod_ModIsEnabled(int inIdVersion, int inIdMod, bool inValue)
        {
            Data.Versions[inIdVersion].Mods[inIdMod].IsEnabled = inValue;
        }
        // Dive.
        public void Settings_Save_Dive_Seed(uint inValue)
        {
            // Internal Setting.
            SettingsCurrent.Dive.Seed = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Dive_Seed(inValue);
            File_Apache24_Write_Dives(inValue);
        }
        public void Settings_Save_Dive_LostDives(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Dive.LostDives = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Dive_LostDives(inValue);
        }
        public void Settings_Save_Dive_Search(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Dive.Search = inValue;
        }
        public void Settings_Save_Dive_SelectedId(int inIdDive)
        {
            // Internal Setting.
            SettingsCurrent.Dive.SelectedId = inIdDive;
            string numberDive = Data_Dives_Get_Number(SettingsCurrent.Version.SelectedId, inIdDive);
            SettingsCurrent.Dive.SelectedNumber = numberDive;
            // External Setting.
            File_App_WrtKey_DDE_Dive_SelectedNumber(numberDive);
        }
        // Event.
        public void Settings_Save_Event_Command(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Event.Command = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Event_Command(inValue);
            File_Apache24_Write_Events(inValue);
        }
        public void Settings_Save_Event_FreeBeers(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Event.FreeBeers = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Event_FreeBeers(inValue);
            File_Apache24_Write_FreeBeers(inValue);
        }
        public void Settings_Save_Event_LostEvents(bool inValue)
        {
            // Internal Setting.
            SettingsCurrent.Event.LostEvents = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Event_LostEvents(inValue);
        }
        public void Settings_Save_Event_Search(string inValue)
        {
            // Internal Setting.
            SettingsCurrent.Event.Search = inValue;
        }
        public void Settings_Save_Event_SelectedId(int inIdEvent)
        {
            // Internal Setting.
            SettingsCurrent.Event.SelectedId = inIdEvent;
            string numberEvent = Data_Events_Get_Number(SettingsCurrent.Version.SelectedId, inIdEvent);
            SettingsCurrent.Event.SelectedNumber = numberEvent;
            // External Setting.
            File_App_WrtKey_DDE_Event_SelectedNumber(numberEvent);
        }
        // Assignment.
        public void Settings_Save_Assignment_Seed(uint inValue)
        {
            // Internal Setting.
            SettingsCurrent.Assignment.Seed = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Assignment_Seed(inValue);
            File_Apache24_Write_Assignments(inValue);
        }
        // Common.
        public void Settings_Save_Common_PosX(double inValue)
        {
            // Internal Setting.
            SettingsCurrent.Common.PosX = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Common_PosX(inValue);
        }
        public void Settings_Save_Common_PosY(double inValue)
        {
            // Internal Setting.
            SettingsCurrent.Common.PosY = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Common_PosY(inValue);
        }
        public void Settings_Save_Common_SizeX(double inValue)
        {
            // Internal Setting.
            SettingsCurrent.Common.SizeX = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Common_SizeX(inValue);
        }
        public void Settings_Save_Common_SizeY(double inValue)
        {
            // Internal Setting.
            SettingsCurrent.Common.SizeY = inValue;
            // External Setting.
            File_App_WrtKey_DDE_Common_SizeY(inValue);
        }
        #endregion
        #region Settings Load
        public void Settings_Load_All()
        {
            //
            Settings_Load_Services_IP();
            Settings_Load_Services_ChangeRedirects();
            Settings_Load_Services_ChangeCertificates();
            Settings_Load_Services_StartServer();
            //
            Settings_Load_Version_PathSimple();
            Settings_Load_Version_PathComplex();
            Settings_Load_Version_Branch();
            Settings_Load_Version_PlayerId();
            Settings_Load_Version_PlayerName();
            Settings_Load_Version_Command();
            Settings_Load_Version_SelectedId();
            //
            Settings_Load_Mods_EnabledMltVersion();
            //
            Settings_Load_Dive_LostDives();
            Settings_Load_Dive_Seed();
            Settings_Load_Dive_SelectedId();
            //
            Settings_Load_Event_Command();
            Settings_Load_Event_FreeBeers();
            Settings_Load_Event_LostEvents();
            Settings_Load_Event_SelectedId();
            //
            Settings_Load_Assignment_Seed();
            //
            Settings_Load_Common_PosX();
            Settings_Load_Common_PosY();
            Settings_Load_Common_SizeX();
            Settings_Load_Common_SizeY();
        }
        // Services.
        public void Settings_Load_Services_IP()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("IP", "Services");
            if (value != null)
            {
                SettingsCurrent.Services.IP = value;
            }
            else
            {
                File_App_WrtKey_DDE_Services_IP(SettingsDefault.Services.IP);
            }
        }
        public void Settings_Load_Services_ChangeRedirects()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("ChangeRedirects", "Services");
            if (value != null)
            {
                SettingsCurrent.Services.ChangeRedirects = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Services_ChangeRedirects(SettingsDefault.Services.ChangeRedirects);
            }
        }
        public void Settings_Load_Services_ChangeCertificates()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("ChangeCertificates", "Services");
            if (value != null)
            {
                SettingsCurrent.Services.ChangeCertificates = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Services_ChangeCertificates(SettingsDefault.Services.ChangeCertificates);
            }
        }
        public void Settings_Load_Services_StartServer()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("StartServer", "Services");
            if (value != null)
            {
                SettingsCurrent.Services.StartServer = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Services_StartServer(SettingsDefault.Services.StartServer);
            }
        }
        // Version.
        public void Settings_Load_Version_PathSimple()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("Path", "Version");
            if (value != null)
            {
                SettingsCurrent.Version.Path = value;
            }
            else
            {
                File_App_WrtKey_DDE_Version_Path(SettingsDefault.Version.Path);
            }
        }
        public void Settings_Load_Version_PathComplex()
        {
            Directory.CreateDirectory(PathFoldSavesVersions);
            string[] pathFiles = Directory.GetFiles(PathFoldSavesVersions, "*.ini", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                for (int i = 0; i < pathFiles.Length; i++)
                {
                    string numberVersion = Path.GetFileNameWithoutExtension(pathFiles[i]);
                    // Check if data for version exists.
                    int idVersion = Data_Versions_Get_Id(numberVersion);
                    if (idVersion >= 0)
                    {
                        FileINI fileINI = new FileINI(pathFiles[i]);
                        string? path = fileINI.ReadKeyString("Path", "SaveVersion");
                        if (path != null)
                        {
                            Data.Versions[idVersion].Path = path; // No need to validate path, because it will be validated on game launch.
                        }
                        else
                        {
                            File.Delete(pathFiles[i]);
                        }
                    }
                    else
                    {
                        File.Delete(pathFiles[i]);
                    }
                }
            }
        }
        public void Settings_Load_Version_Branch()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("Branch", "Version");
            if (value != null)
            {
                SettingsCurrent.Version.Branch = value;
            }
            else
            {
                File_App_WrtKey_DDE_Version_Branch(SettingsDefault.Version.Branch);
            }
        }
        public void Settings_Load_Version_PlayerId()
        {
            if (File.Exists(PathFileGSEUserSteamId) == true)
            {
                string[] lines = File.ReadAllLines(PathFileGSEUserSteamId);
                // If file will be empty, it's length will be null and it will not load empty string.
                if (lines.Length == 1)
                {
                    SettingsCurrent.Version.PlayerId = lines[0];
                }
                else
                {
                    File_GSE_WrtFile_UserSteamId(SettingsDefault.Version.PlayerId);
                }
            }
            else
            {
                File_GSE_WrtFile_UserSteamId(SettingsDefault.Version.PlayerId);
            }
        }
        public void Settings_Load_Version_PlayerName()
        {
            if (File.Exists(PathFileGSEAccountName) == true)
            {
                string[] lines = File.ReadAllLines(PathFileGSEAccountName);
                // If file will be empty, it's length will be null and it will not load empty string.
                if (lines.Length == 1)
                {
                    SettingsCurrent.Version.PlayerName = lines[0];
                }
                else
                {
                    File_GSE_WrtFile_AccountName(SettingsDefault.Version.PlayerName);
                }
            }
            else
            {
                File_GSE_WrtFile_AccountName(SettingsDefault.Version.PlayerName);
            }
        }
        public void Settings_Load_Version_Command()
        {
            string pathFile = PathFileGSEColdClientLoader;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("ExeCommandLine", "SteamClient");
            if (value != null)
            {
                SettingsCurrent.Version.Command = value;
            }
            else
            {
                File_GSE_WrtKey_CCL_SteamClient_ExeCommandLine(SettingsDefault.Version.Command);
            }
        }
        public void Settings_Load_Version_SelectedId()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("SelectedNumber", "Version");
            if (value != null)
            {
                int selectedId = Data_Versions_Get_Id(value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Version.SelectedId = selectedId;
                    SettingsCurrent.Version.SelectedNumber = value;
                }
                else
                {
                    File_App_WrtKey_DDE_Version_SelectedNumber(SettingsDefault.Version.SelectedNumber);
                }
            }
            else
            {
                File_App_WrtKey_DDE_Version_SelectedNumber(SettingsDefault.Version.SelectedNumber);
            }
        }
        // Mod.
        public void Settings_Load_Mods_EnabledMltVersion()
        {
            for (int i = 0; i < Data.Versions.Count; i++)
            {
                Settings_Load_Mods_EnabledSngVersion(i);
            }
        }
        public void Settings_Load_Mods_EnabledSngVersion(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                {
                    Data.Versions[inIdVersion].Mods[i].IsEnabled = false;
                }
                if (File.Exists(Data.Versions[inIdVersion].Path) == true)
                {
                    // "Path.GetFullPath" is used to convert "..\\" to go up the root.
                    // Notice that three "..\\" are used. One to remove "FSD-Win64-Shipping.exe", others to go up the root.
                    string pathFile = Path.GetFullPath(Path.Combine(Data.Versions[inIdVersion].Path, "..\\..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                    FileINI fileINI = new FileINI(pathFile);
                    for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                    {
                        bool? value = fileINI.ReadKeyBool(Data.Versions[inIdVersion].Mods[i].Number, "/Script/FSD.UserGeneratedContent");
                        if (value != null)
                        {
                            Data.Versions[inIdVersion].Mods[i].IsEnabled = value.Value;
                        }
                    }
                }
            }
        }
        // Dive.
        public void Settings_Load_Dive_Seed()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            uint? value = fileINI.ReadKeyUInt("Seed", "Dive");
            if (value != null)
            {
                SettingsCurrent.Dive.Seed = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Dive_Seed(SettingsDefault.Dive.Seed);
            }
        }
        public void Settings_Load_Dive_LostDives()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("LostDives", "Dive");
            if (value != null)
            {
                SettingsCurrent.Dive.LostDives = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Dive_LostDives(SettingsDefault.Dive.LostDives);
            }
        }
        public void Settings_Load_Dive_SelectedId()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("SelectedNumber", "Dive");
            if (value != null)
            {
                int selectedId = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Dive.SelectedId = selectedId;
                    SettingsCurrent.Dive.SelectedNumber = value;
                }
                else
                {
                    File_App_WrtKey_DDE_Dive_SelectedNumber(SettingsDefault.Dive.SelectedNumber);
                }
            }
            else
            {
                File_App_WrtKey_DDE_Dive_SelectedNumber(SettingsDefault.Dive.SelectedNumber);
            }
        }
        // Event.
        public void Settings_Load_Event_Command()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("Command", "Event");
            if (value != null)
            {
                SettingsCurrent.Event.Command = value;
            }
            else
            {
                File_App_WrtKey_DDE_Event_Command(SettingsDefault.Event.Command);
            }
        }
        public void Settings_Load_Event_FreeBeers()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("FreeBeers", "Event");
            if (value != null)
            {
                SettingsCurrent.Event.FreeBeers = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Event_FreeBeers(SettingsDefault.Event.FreeBeers);
            }
        }
        public void Settings_Load_Event_LostEvents()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            bool? value = fileINI.ReadKeyBool("LostEvents", "Event");
            if (value != null)
            {
                SettingsCurrent.Event.LostEvents = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Event_LostEvents(SettingsDefault.Event.LostEvents);
            }
        }
        public void Settings_Load_Event_SelectedId()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            string? value = fileINI.ReadKeyString("SelectedNumber", "Event");
            if (value != null)
            {
                int selectedId = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Event.SelectedId = selectedId;
                    SettingsCurrent.Event.SelectedNumber = value;
                }
                else
                {
                    File_App_WrtKey_DDE_Event_SelectedNumber(SettingsDefault.Event.SelectedNumber);
                }
            }
            else
            {
                File_App_WrtKey_DDE_Event_SelectedNumber(SettingsDefault.Event.SelectedNumber);
            }
        }
        // Assignment.
        public void Settings_Load_Assignment_Seed()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            uint? value = fileINI.ReadKeyUInt("Seed", "Assignment");
            if (value != null)
            {
                SettingsCurrent.Assignment.Seed = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Assignment_Seed(SettingsDefault.Assignment.Seed);
            }
        }
        // Common.
        public void Settings_Load_Common_PosX()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            double? value = fileINI.ReadKeyDouble("PosX", "Common");
            if (value != null)
            {
                SettingsCurrent.Common.PosX = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Common_PosX(SettingsDefault.Common.PosX);
            }
        }
        public void Settings_Load_Common_PosY()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            double? value = fileINI.ReadKeyDouble("PosY", "Common");
            if (value != null)
            {
                SettingsCurrent.Common.PosY = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Common_PosY(SettingsDefault.Common.PosY);
            }
        }
        public void Settings_Load_Common_SizeX()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            double? value = fileINI.ReadKeyDouble("SizeX", "Common");
            if (value != null)
            {
                SettingsCurrent.Common.SizeX = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Common_SizeX(SettingsDefault.Common.SizeX);
            }
        }
        public void Settings_Load_Common_SizeY()
        {
            string pathFile = PathFileSettings;
            FileINI fileINI = new FileINI(pathFile);
            double? value = fileINI.ReadKeyDouble("SizeY", "Common");
            if (value != null)
            {
                SettingsCurrent.Common.SizeY = value.Value;
            }
            else
            {
                File_App_WrtKey_DDE_Common_SizeY(SettingsDefault.Common.SizeY);
            }
        }
        #endregion
        #region Game
        public void Game_Save_LaunchEssentials(int inIdVersion)
        {
            File_Game_WrtKey_E_ENS_VerifyPeer(inIdVersion, "0");
            //
            File_Game_DelSec_GUS_SFSDUGC(inIdVersion);
            //
            File_Game_WrtKey_GUS_SFSDUGC_ModsAreEnabled(inIdVersion);
            File_Game_WrtKey_GUS_SFSDUGC_CheckGameversion(inIdVersion, "False");
            File_Game_WrtKey_GUS_SFSDUGC_CurrentBranchName(inIdVersion, SettingsCurrent.Version.Branch);
        }
        public bool? Game_Check_Path(string inPath)
        {
            bool? check = null;
            if (File.Exists(inPath) == true)
            {
                if (inPath.EndsWith("FSD-Win64-Shipping.exe") == true)
                {
                    check = true;
                }
                else
                {
                    check = false;
                }
            }
            return check;
        }
        #endregion
        #region GoldbergSteamEmulator
        public void GSE_Save_LaunchEssentials(int inIdVersion)
        {
            // ColdClientLoader.ini.
            File_GSE_WrtKey_CCL_SteamClient_AppId("548430");
            FileINI fileINI = new FileINI(PathFileGSEColdClientLoader);
            string path = SettingsCurrent.Version.Path;
            if (inIdVersion >= 0)
            {
                path = Data.Versions[inIdVersion].Path;
            }
            fileINI.WriteKey("Exe", "SteamClient", path);
            fileINI.WriteKey("ExeRunDir", "SteamClient", ".");
            string pathFileSCDLL = Path.Combine(PathFoldGSE, "steamclient.dll");
            fileINI.WriteKey("SteamClientDll", "SteamClient", pathFileSCDLL);
            string pathFileSC64DLL = Path.Combine(PathFoldGSE, "steamclient64.dll");
            fileINI.WriteKey("SteamClient64Dll", "SteamClient", pathFileSC64DLL);
            // local_save.txt.
            File_GSE_WrtFile_LocalSave();
            // account_name.txt.
            File_GSE_WrtFile_AccountName(SettingsCurrent.Version.PlayerName);
            // user_steam_id.txt.
            File_GSE_WrtFile_UserSteamId(SettingsCurrent.Version.PlayerId);
        }
        public void GSE_Launch(int inIdVersion)
        {
            GSE_Save_LaunchEssentials(inIdVersion);
            Game_Save_LaunchEssentials(inIdVersion);
            ModIO_Save_LaunchEssentials(inIdVersion);
            //
            if (File.Exists(PathFileGSESteamClient_Loader) == true)
            {
                Process.Start(PathFileGSESteamClient_Loader);
            }
        }
        #endregion
        #region ModIO
        public void ModIO_Save_LaunchEssentials(int inIdVersion)
        {
            File_ModIO_WrtFile_State(inIdVersion);
            File_ModIO_WrtFile_GlobalSettings(inIdVersion);
            File_ModIO_WrtFile_User(inIdVersion);
        }
        #endregion
        #region Steam
        public void Steam_OpenConsole()
        {
            Process.Start("explorer.exe", "steam://nav/console");
        }
        public bool Steam_CheckRunning()
        {
            bool value = false;
            Process[] processes = Process.GetProcessesByName("Steam");
            if (processes.Length > 0)
            {
                value = true;
            }
            return value;
        }
        #endregion
        #region Window
        public void Window_Load()
        {
            TmrWindPos.Tick += TmrWindowPos_Tick;
            TmrWindPos.Interval = TimeSpan.FromSeconds(0.5);
            TmrWindSize.Tick += TmrWindowSize_Tick;
            TmrWindSize.Interval = TimeSpan.FromSeconds(0.5);
        }
        public void Window_Set_Position(double inPosX, double inPosY)
        {
            this.Left = inPosX;
            this.Top = inPosY;
        }
        public void Window_Set_Size(double inSizeX, double inSizeY)
        {
            this.Width = inSizeX;
            this.Height = inSizeY;
        }
        public void TmrWindowPos_Start()
        {
            TmrWindPos.Start();
        }
        public void TmrWindowPos_Stop()
        {
            TmrWindPos.Stop();
        }
        public void TmrWindowPos_Update()
        {
            TmrWindowPos_Stop();
            TmrWindowPos_Start();
        }
        public void TmrWindowSize_Start()
        {
            TmrWindSize.Start();
        }
        public void TmrWindowSize_Stop()
        {
            TmrWindSize.Stop();
        }
        public void TmrWindowSize_Update()
        {
            TmrWindowSize_Stop();
            TmrWindowSize_Start();
        }
        #endregion
        //
        #region String
        public bool String_Check_NumberVersion(string inNumber, out string[] outNumberParts)
        {
            outNumberParts = inNumber.Split('.');
            if (
                outNumberParts.Length == 4
                &&
                int.TryParse(outNumberParts[0], out int number0) == true && number0 >= 0
                &&
                int.TryParse(outNumberParts[1], out int number1) == true && number1 >= 0
                &&
                int.TryParse(outNumberParts[2], out int number2) == true && number2 >= 0
                &&
                int.TryParse(outNumberParts[3], out int number3) == true && number3 >= 0
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool String_Check_NumberMod(string inNumber)
        {
            if (int.TryParse(inNumber, out int number) == true && number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool String_Check_NumberDive(string inNumber)
        {
            string[] numberParts = inNumber.Split('.');
            if (
                numberParts.Length == 1
                &&
                int.TryParse(numberParts[0], out int numberA0) == true && numberA0 > 0
                ||
                numberParts.Length == 2
                &&
                int.TryParse(numberParts[0], out int numberB0) == true && numberB0 > 0
                &&
                int.TryParse(numberParts[1], out int numberB1) == true && numberB1 > 0
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool String_Check_NumberEvent(string inNumber)
        {
            if (int.TryParse(inNumber, out int number) == true && number > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void String_ReduceTo_Int32(string inStr, int inPos, out string outStr, out int outPos)
        {
            outPos = inPos;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char symbol in inStr)
            {
                if (symbol == '-' && stringBuilder.Length == 0)
                {
                    stringBuilder.Append(symbol);
                }
                else if (char.IsDigit(symbol) == true)
                {
                    // If it is second symbol.
                    if (stringBuilder.Length == 1)
                    {
                        if (stringBuilder[0] == '-')
                        {
                            if (symbol != '0')
                            {
                                stringBuilder.Append(symbol);
                            }
                        }
                        else if (stringBuilder[0] == '0')
                        {
                            stringBuilder.Remove(0, 1);
                            stringBuilder.Append(symbol);
                        }
                        else
                        {
                            stringBuilder.Append(symbol);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(symbol);
                    }
                }
                else if (inPos > stringBuilder.Length)
                {
                    // Move selection to correct position.
                    // If not moved, position will be shift to the start, after entering incorrect symbol.
                    outPos = outPos - 1;
                }
            }
            bool strCheck = long.TryParse(stringBuilder.ToString(), out long strChecked);
            if (strCheck == true && strChecked < -2147483648)
            {
                outStr = "-2147483648";
            }
            else if (strCheck == true && strChecked > 2147483647)
            {
                outStr = "2147483647";
            }
            else
            {
                outStr = stringBuilder.ToString();
            }
        }
        public void String_ReduceTo_UInt32(string inStr, int inPos, out string outStr, out int outPos)
        {
            outPos = inPos;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char symbol in inStr)
            {
                if (char.IsDigit(symbol) == true)
                {
                    // Note that ' is used instead of ", because it is a char, not a string.
                    if (stringBuilder.Length == 1)
                    {
                        if (stringBuilder[0] == '0')
                        {
                            stringBuilder.Remove(0, 1);
                            stringBuilder.Append(symbol);
                        }
                        else
                        {
                            stringBuilder.Append(symbol);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(symbol);
                    }
                }
                else if (inPos > stringBuilder.Length)
                {
                    // Move selection to correct position.
                    // If not moved, position will be shift to the start, after entering incorrect symbol.
                    outPos = outPos - 1;
                }
            }
            bool strCheck = ulong.TryParse(stringBuilder.ToString(), out ulong strChecked);
            if (strCheck == true && strChecked > 4294967295)
            {
                outStr = "4294967295";
            }
            else
            {
                outStr = stringBuilder.ToString();
            }
        }
        public void String_ReduceTo_Manifest(string inStr, int inPos, out string outStr, out int outPos)
        {
            outPos = inPos;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char symbol in inStr)
            {
                if (stringBuilder.Length < 19)
                {
                    if (char.IsDigit(symbol) == true)
                    {
                        stringBuilder.Append(symbol);
                    }
                }
                else if (inPos > stringBuilder.Length)
                {
                    // Move selection to correct position.
                    // If not moved, position will be shift to the start, after entering incorrect symbol.
                    outPos = outPos - 1;
                }
            }
            outStr = stringBuilder.ToString();
        }
        public string String_FormatTo_Literal(string inText)
        {
            return SymbolDisplay.FormatLiteral(inText, false);
        }
        #endregion
        #region Number
        public uint Number_Generate_Seed()
        {
            // Generate value from 0 to 4294967295.
            uint seed = (uint)new Random().Next(-int.MaxValue, int.MaxValue);
            return seed;
        }
        public static DateTime Number_ConvertTo_DateTime(double inUnixTimeStamp)
        {
            DateTime outDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            outDateTime = outDateTime.AddSeconds(inUnixTimeStamp).ToLocalTime();
            return outDateTime;
        }
        #endregion
        #region Server
        public void Server_Redirects_Add()
        {
            Directory.CreateDirectory(PathFoldApache24Certificates);
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Check if files have correct extention.
                List<string> pathFilesChecked = new List<string>();
                for (int i = 0; i < pathFiles.Length; i++)
                {
                    string ext = Path.GetExtension(pathFiles[i]);
                    if (ext == ".crt" || ext == ".txt")
                    {
                        pathFilesChecked.Add(pathFiles[i]);
                    }
                }
                if (pathFilesChecked.Count > 0)
                {
                    if (File.Exists(PathFileWindowsHosts) == false)
                    {
                        // `File.Create` command causes crash.
                        File.WriteAllText(PathFileWindowsHosts, "");
                    }
                    string[] lines = File.ReadAllLines(PathFileWindowsHosts);

                    //
                    int count = 0;
                    // Check if lines exist (empty file will have 0 length). No need to move to next new line when file is empty.
                    if (lines.Length > 0)
                    {
                        // Check if file has empty line at the end. `ReadAllText` is used, beacause `lines[lines.Length - 1]` will not detect empty line at the end.
                        if (File.ReadAllText(PathFileWindowsHosts).EndsWith("\r\n") == true)
                        {
                            count = 1;
                        }
                        else
                        {
                            count = 2;
                        }
                    }
                    // Add empty lines.
                    for (int i = 0; i < count; i++)
                    {
                        File.AppendAllText(PathFileWindowsHosts, Environment.NewLine);
                    }
                    // Add label for app redirects.
                    File.AppendAllText(PathFileWindowsHosts, "#DeepDiveEmulator Redirects" + Environment.NewLine);
                    // Add redirects.
                    for (int i = 0; i < pathFilesChecked.Count; i++)
                    {
                        // Define redirect name.
                        string hostName = Path.GetFileNameWithoutExtension(pathFilesChecked[i]);
                        // Define command.
                        string command = SettingsCurrent.Services.IP + " " + hostName;
                        // Write command down at the end of all lines.
                        File.AppendAllText(PathFileWindowsHosts, command + Environment.NewLine);
                    }
                }
            }
        }
        public void Server_Redirects_Remove()
        {
            if (File.Exists(PathFileWindowsHosts) == true)
            {
                string[] lines = File.ReadAllLines(PathFileWindowsHosts);
                List<string> tempLines = new List<string>();

                // Remove line `#DeepDiveEmulator Redirects`.
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("#DeepDiveEmulator Redirects") == false)
                    {
                        tempLines.Add(lines[i]);
                    }
                    else
                    {
                        // Check if previous line is empty.
                        if (tempLines.Count > 0 && tempLines[tempLines.Count - 1] == "")
                        {
                            // Remove previous line, because it was added with redirects for space.
                            tempLines.RemoveAt(tempLines.Count - 1);
                        }
                    }
                }
                // Update array after the change, because comment position may be different (user defined).
                lines = tempLines.ToArray();
                tempLines.Clear();

                // Remove redirects.
                Directory.CreateDirectory(PathFoldApache24Certificates);
                string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "", SearchOption.TopDirectoryOnly);
                if (pathFiles.Length > 0)
                {
                    // Check if files have correct extention.
                    List<string> pathFilesChecked = new List<string>();
                    for (int i = 0; i < pathFiles.Length; i++)
                    {
                        string ext = Path.GetExtension(pathFiles[i]);
                        if (ext == ".crt" || ext == ".txt")
                        {
                            pathFilesChecked.Add(pathFiles[i]);
                        }
                    }
                    if (pathFilesChecked.Count > 0)
                    {
                        for (int iPathFileChecked = 0; iPathFileChecked < pathFilesChecked.Count; iPathFileChecked++)
                        {
                            // Define redirect name.
                            string hostName = Path.GetFileNameWithoutExtension(pathFilesChecked[iPathFileChecked]);
                            //
                            for (int iLine = 0; iLine < lines.Length; iLine++)
                            {
                                if (lines[iLine].Contains(hostName) == false)
                                {
                                    tempLines.Add(lines[iLine]);
                                }
                            }
                            // Update array right after the change, because order of the redirects may be different (user defined) compare to order of files in the folder.
                            lines = tempLines.ToArray();
                            tempLines.Clear();
                        }
                    }
                }
                // Write down all lines, without redirects.
                File.WriteAllLines(PathFileWindowsHosts, lines);
            }
        }
        public bool Server_Redirects_Check()
        {
            Directory.CreateDirectory(PathFoldApache24Certificates);
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                List<string> pathFilesChecked = new List<string>();
                for (int i = 0; i < pathFiles.Length; i++)
                {
                    string ext = Path.GetExtension(pathFiles[i]);
                    if (ext == ".crt" || ext == ".txt")
                    {
                        pathFilesChecked.Add(pathFiles[i]);
                    }
                }
                if (pathFilesChecked.Count > 0)
                {
                    if (File.Exists(PathFileWindowsHosts) == true)
                    {
                        string[] lines = File.ReadAllLines(PathFileWindowsHosts);
                        if (lines.Length > 0)
                        {
                            for (int iPathFileChecked = 0; iPathFileChecked < pathFilesChecked.Count; iPathFileChecked++)
                            {
                                // Define redirect name.
                                string hostName = Path.GetFileNameWithoutExtension(pathFilesChecked[iPathFileChecked]);
                                // Define command without spaces.
                                string command = SettingsCurrent.Services.IP + hostName;
                                //
                                for (int iLine = 0; iLine < lines.Length; iLine++)
                                {
                                    // Check if line, without any spaces will be equal to the command.
                                    if (Regex.Replace(lines[iLine], "\\s+", "") == command)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (iLine == lines.Length - 1)
                                        {
                                            // One required redirect dosen't exist.
                                            return false;
                                        }
                                    }
                                }
                            }
                            // All required redirects exist.
                            return true;
                        }
                    }
                }
            }
            // Files don't exist or `hosts` file doesn't exist or `hosts` file is empty.
            return false;
        }
        public void Server_Redirects_OpenFolder()
        {
            Process.Start("explorer.exe", PathFoldWindowsETC);
        }
        public void Server_Certificates_Add()
        {
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                //X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser); // !!! Kept for potentional windows protection fix.
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                //
                foreach (string pathFile in pathFiles)
                {
                    X509Certificate2 certificate = new X509Certificate2(pathFile);
                    store.Add(certificate);
                }
                //
                store.Close();
            }
        }
        public void Server_Certificates_Remove()
        {
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                //X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser); // !!! Kept for potentional windows protection fix.
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite | OpenFlags.IncludeArchived);
                //
                foreach (string pathFile in pathFiles)
                {
                    // Define certificate name.
                    // Using name for removing is better, because you can remove old app version certificates.
                    string name = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                    //
                    X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindBySubjectName, name, false);
                    foreach (var certificate in certificates)
                    {
                        store.Remove(certificate);
                    }
                }
                //
                store.Close();
            }
        }
        public bool Server_Certificates_Check()
        {
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Count for the ammount of current certificates with required SHA1.
                int count = 0;
                //
                //X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser); // !!! Kept for potentional windows protection fix.
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                //
                foreach (string pathFile in pathFiles)
                {
                    // Define certificate name.
                    string name = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                    // Get certificates with this name. 
                    X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindBySubjectName, name, false);
                    if (certificates.Count > 0)
                    {
                        // Get requred certificate SHA1.
                        string sha1Required = X509Certificate.CreateFromCertFile(pathFile).GetCertHashString();
                        //
                        foreach (X509Certificate certificate in certificates)
                        {
                            // Get current certificate SHA1.
                            string sha1Current = certificate.GetCertHashString();
                            if (sha1Required == sha1Current)
                            {
                                // Increase the ammount of current certificates with required SHA1.
                                count = count + 1;
                                // Required certificate was found.
                                break;
                            }
                        }
                    }
                    else
                    {
                        // One required certificate is not installed.
                        return false;
                    }
                }
                //
                store.Close();
                // Check if ammount of current certificates is equal to amount of files.
                if (count == pathFiles.Length)
                {
                    // All required certificates are installed.
                    return true;
                }
                else
                {
                    // One or more required certificates are not installed.
                    return false;
                }
            }
            else
            {
                // Folder does not contain certificates to check with.
                return false;
            }
        }
        public void Server_Certificates_OpenFolder()
        {
            Process.Start("explorer.exe", PathFoldApache24Certificates);
        }
        public void Server_Install()
        {
            if (File.Exists(PathFileHTTPDConfig) == true)
            {
                List<string> linesTemp = new List<string>();
                string[] lines = File.ReadAllLines(PathFileHTTPDConfig);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("Define SRVROOT", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        string command = "Define SRVROOT \"" + PathFoldApache24 + "\"";
                        linesTemp.Add(command);
                    }
                    else
                    {
                        linesTemp.Add(lines[i]);
                    }
                }
                File.WriteAllLines(PathFileHTTPDConfig, linesTemp);
            }
        }
        public void Server_Start()
        {
            // Write down required path for server to work.
            Server_Install();
            //
            if (File.Exists(PathFileHTTPDEXE) == true)
            {
                Process process = new Process();
                process.StartInfo.FileName = PathFileHTTPDEXE;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
            }
        }
        public void Server_Stop()
        {
            Process[] processes = Process.GetProcessesByName("httpd");
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
        public bool Server_Check()
        {
            Process[] processes = Process.GetProcessesByName("httpd");
            if (processes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region Tabs
        // Tab Services.
        public void TabServices_Load()
        {
            // IP Address.
            TBoxSvcsRedsIP.Text = SettingsCurrent.Services.IP;
            // Add/Remove Website Redirects on launch/close.
            CBoxSvcsRedsChange.IsChecked = SettingsCurrent.Services.ChangeRedirects;
            if (SettingsCurrent.Services.ChangeRedirects == true)
            {
                Server_Redirects_Remove();
                Server_Redirects_Add();
            }
            // Website Redirects are added.
            CBoxSvcsRedsAdded_Enable(Server_Redirects_Check());
            // Timer Website Redirects are added.
            TmrSvcsStatusRedirects.Tick += TmrSvcsStatusRedirects_Tick;
            TmrSvcsStatusRedirects.Interval = TimeSpan.FromSeconds(2.5);
            // Add/Remove Website Certificates on launch/close.
            CBoxSvcsCertsChange.IsChecked = SettingsCurrent.Services.ChangeCertificates;
            if (SettingsCurrent.Services.ChangeCertificates == true)
            {
                Server_Certificates_Remove();
                Server_Certificates_Add();
            }
            // Website Certificates are added.
            CBoxSvcsCertsAdded_Enable(Server_Certificates_Check());
            // Timer Website Certificates are added.
            TmrSvcsStatusCertificates.Tick += TmrSvcsStatusCertificates_Tick;
            TmrSvcsStatusCertificates.Interval = TimeSpan.FromSeconds(2.5);
            // Start Server on launch.
            CBoxSvcsServChange.IsChecked = SettingsCurrent.Services.StartServer;
            if (SettingsCurrent.Services.StartServer == true)
            {
                Server_Start();
            }
            // Server is running.
            CBoxSvcsServRunning_Enable(Server_Check());
            // Timer Server is running.
            TmrSvcsStatusServer.Tick += TmrSvcsStatusServer_Tick;
            TmrSvcsStatusServer.Interval = TimeSpan.FromSeconds(2.5);
            // Timer Services Status.
            TmrSvcsStatus.Tick += TmrSvcsStatus_Tick;
            TmrSvcsStatus.Interval = TimeSpan.FromSeconds(10);
            TmrServicesCheck_Start();
        }
        public void CBoxSvcsRedsAdded_Enable(bool inEnable)
        {
            if (inEnable == true)
            {
                CBoxSvcsRedsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxSvcsRedsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void CBoxSvcsCertsAdded_Enable(bool inEnable)
        {
            if (inEnable == true)
            {
                CBoxSvcsCertsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxSvcsCertsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void CBoxSvcsServRunning_Enable(bool inEnable)
        {
            if (inEnable == true)
            {
                CBoxSvcsServRunning.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxSvcsServRunning.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void TmrServicesCheck_Start()
        {
            TmrSvcsStatus.Start();
        }
        public void TmrRedsAdded_Update()
        {
            TmrRedsAdded_Stop();
            TmrRedsAdded_Start();
        }
        public void TmrRedsAdded_Start()
        {
            TmrSvcsStatusRedirects.Start();
        }
        public void TmrRedsAdded_Stop()
        {
            TmrSvcsStatusRedirects.Stop();
        }
        public void TmrCertsAdded_Update()
        {
            TmrCertsAdded_Stop();
            TmrCertsAdded_Start();
        }
        public void TmrCertsAdded_Start()
        {
            TmrSvcsStatusCertificates.Start();
        }
        public void TmrCertsAdded_Stop()
        {
            TmrSvcsStatusCertificates.Stop();
        }
        public void TmrServRunning_Update()
        {
            TmrServRunning_Stop();
            TmrServRunning_Start();
        }
        public void TmrServRunning_Start()
        {
            TmrSvcsStatusServer.Start();
        }
        public void TmrServRunning_Stop()
        {
            TmrSvcsStatusServer.Stop();
        }
        // Tab Versions.
        public void TabVersions_Load()
        {
            CBoxVerBranch.ItemsSource = Data.VersionParameters.Branches;
            CBoxVerBranch.Text = SettingsCurrent.Version.Branch;
            //
            TBoxVersPlrId.Text = SettingsCurrent.Version.PlayerId;
            //
            TBoxVersPlrName.Text = SettingsCurrent.Version.PlayerName;
            //
            TBoxVersCmdLnch.Text = SettingsCurrent.Version.Command;
            //
            TmrVersSearch.Tick += TmrVersSearch_Tick;
            TmrVersSearch.Interval = TimeSpan.FromSeconds(0.5);
            //
            VListVers.ItemsSource = SourceVlistVersions;
            VListVers_SelectId(SettingsCurrent.Version.SelectedId);
            VListVers_Fill();
        }
        public void TmrVersSearch_Start()
        {
            TmrVersSearch.Start();
        }
        public void TmrVersSearch_Stop()
        {
            TmrVersSearch.Stop();
        }
        public void TmrVersSearch_Update()
        {
            TmrVersSearch_Stop();
            TmrVersSearch_Start();
        }
        public void VListVers_Fill()
        {
            SourceVlistVersions.Clear();
            List<DataVersion> dataVersions = Data_Versions_Get_Versions(SettingsCurrent.Version.Search);
            dataVersions = Data_Versions_Sort_Versions(dataVersions);
            for (int i = 0; i < dataVersions.Count; i++)
            {
                // Create item.
                SrcVListVersion srcVListVersion = new SrcVListVersion();
                srcVListVersion.Number = dataVersions[i].Number;
                srcVListVersion.Name = dataVersions[i].Name;
                srcVListVersion.BrushBack = dataVersions[i].Brush;
                // Add item.
                SourceVlistVersions.Add(srcVListVersion);
            }
            VListVers.SelectedIndex = Data_Versions_Get_Id(SettingsCurrent.Version.SelectedNumber, dataVersions);
        }
        public void VListVers_SelectId(int inIdVersion)
        {
            int idVersionOld = SettingsCurrent.Version.SelectedId;
            //
            Settings_Save_Version_SelectedId(inIdVersion);
            // Internal.
            if (inIdVersion >= 0)
            {
                TBoxVersPath.Text = Data.Versions[inIdVersion].Path;
                BtnVersCmdDepotCopy.IsEnabled = true;
                BtnVersSteamOpen.IsEnabled = true;
            }
            else
            {
                TBoxVersPath.Text = SettingsCurrent.Version.Path;
                BtnVersCmdDepotCopy.IsEnabled = false;
                BtnVersSteamOpen.IsEnabled = false;
            }
            // External.
            //
            if (inIdVersion >= 0)
            {
                BtnModsBackup.IsEnabled = true;
            }
            else
            {
                BtnModsBackup.IsEnabled = false;
            }
            //
            int idMod = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Mod.SelectedNumber);
            VListMods_SelectId(idMod, true);
            VListMods_Fill();
            //
            int idDive = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Dive.SelectedNumber);
            VListDivs_SelectId(idDive, true);
            VListDivs_Fill();
            //
            int idEvent = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Event.SelectedNumber);
            VListEvts_SelectId(idEvent, true);
            VListEvts_Fill();
        }
        // Tab Mods.
        public void TabMods_Load()
        {
            //
            TmrModsSearch.Tick += TmrModsSearch_Tick;
            TmrModsSearch.Interval = TimeSpan.FromSeconds(0.5);
            //
            VListMods.ItemsSource = SourceVlistMods;
            VListMods_Fill();
        }
        public void TmrModsSearch_Start()
        {
            TmrModsSearch.Start();
        }
        public void TmrModsSearch_Stop()
        {
            TmrModsSearch.Stop();
        }
        public void TmrModsSearch_Update()
        {
            TmrModsSearch_Stop();
            TmrModsSearch_Start();
        }
        public void VListMods_Fill()
        {
            SourceVlistMods.Clear();
            if (SettingsCurrent.Version.SelectedId >= 0)
            {
                List<DataMod> dataMods = Data_Mods_Get_Mods(SettingsCurrent.Version.SelectedId, SettingsCurrent.Mod.Search);
                dataMods = Data_Mods_Sort_Mods(dataMods);
                for (int i = 0; i < dataMods.Count; i++)
                {
                    // Create item.
                    SrcVListMod srcVListMod = new SrcVListMod();
                    srcVListMod.Number = dataMods[i].Number;
                    srcVListMod.Name = dataMods[i].Name;
                    srcVListMod.Date = Number_ConvertTo_DateTime(dataMods[i].Time).ToString("yyyy-MM-dd");
                    srcVListMod.IsEnabled = dataMods[i].IsEnabled;
                    srcVListMod.BrushBack = dataMods[i].BrushBack;
                    // Add item.
                    SourceVlistMods.Add(srcVListMod);
                }
                VListMods.SelectedIndex = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Mod.SelectedNumber, dataMods);
            }
        }
        public void VListMods_SelectId(int inIdMod, bool inSkip = false)
        {
            bool change = true;
            int idModOld = SettingsCurrent.Mod.SelectedId;
            //
            if (inSkip == false)
            {
            }
            //
            if (change == true)
            {
                Settings_Save_Mod_SelectedId(inIdMod);
                //
                DataMod dataMod = Data_Mods_Get_Mod(SettingsCurrent.Version.SelectedId, SettingsCurrent.Mod.SelectedId);
                if (inIdMod >= 0)
                {
                    RTBoxModsDescription.Document.Blocks.Clear();
                    RTBoxModsDescription.Document.Blocks.Add(new Paragraph(new Run(dataMod.Description)));
                }
                else
                {
                    RTBoxModsDescription.Document.Blocks.Clear();
                }
            }
        }
        // Tab Dives.
        public void TabDives_Load()
        {
            //
            CBoxDivsLost.IsChecked = SettingsCurrent.Dive.LostDives;
            //
            BoxDivsInfo_Enable(false);
            CBoxDDNorReg.ItemsSource = Data.DiveParameters.Regions;
            CBoxDDNorReg.SelectedIndex = 0;
            CBoxDDNorMisT1.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDNorMisT1.SelectedIndex = 0;
            CBoxDDNorMisT2.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDNorMisT2.SelectedIndex = 0;
            CBoxDDNorMisT3.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDNorMisT3.SelectedIndex = 0;
            CBoxDDNorObjT1.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDNorObjT1.SelectedIndex = 0;
            CBoxDDNorObjT2.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDNorObjT2.SelectedIndex = 0;
            CBoxDDNorObjT3.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDNorObjT3.SelectedIndex = 0;
            CBoxDDNorWar1.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDNorWar1.SelectedIndex = 0;
            CBoxDDNorWar2.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDNorWar2.SelectedIndex = 0;
            CBoxDDNorWar3.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDNorWar3.SelectedIndex = 0;
            CBoxDDNorAno1.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDNorAno1.SelectedIndex = 0;
            CBoxDDNorAno2.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDNorAno2.SelectedIndex = 0;
            CBoxDDNorAno3.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDNorAno3.SelectedIndex = 0;
            CBoxDDEliReg.ItemsSource = Data.DiveParameters.Regions;
            CBoxDDEliReg.SelectedIndex = 0;
            CBoxDDEliMisT1.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDEliMisT1.SelectedIndex = 0;
            CBoxDDEliMisT2.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDEliMisT2.SelectedIndex = 0;
            CBoxDDEliMisT3.ItemsSource = Data.DiveParameters.Missions;
            CBoxDDEliMisT3.SelectedIndex = 0;
            CBoxDDEliObjT1.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDEliObjT1.SelectedIndex = 0;
            CBoxDDEliObjT2.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDEliObjT2.SelectedIndex = 0;
            CBoxDDEliObjT3.ItemsSource = Data.DiveParameters.Objectives;
            CBoxDDEliObjT3.SelectedIndex = 0;
            CBoxDDEliWar1.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDEliWar1.SelectedIndex = 0;
            CBoxDDEliWar2.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDEliWar2.SelectedIndex = 0;
            CBoxDDEliWar3.ItemsSource = Data.DiveParameters.Warnings;
            CBoxDDEliWar3.SelectedIndex = 0;
            CBoxDDEliAno1.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDEliAno1.SelectedIndex = 0;
            CBoxDDEliAno2.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDEliAno2.SelectedIndex = 0;
            CBoxDDEliAno3.ItemsSource = Data.DiveParameters.Anomalies;
            CBoxDDEliAno3.SelectedIndex = 0;
            //
            TmrDivsSearch.Tick += TmrDivsSearch_Tick;
            TmrDivsSearch.Interval = TimeSpan.FromSeconds(0.5);
            //
            VListDivs.ItemsSource = SourceVlistDives;
            VListDivs_SelectId(SettingsCurrent.Dive.SelectedId);
            VListDivs_Fill();
        }
        public void BoxDivsInfo_Enable(bool inputEnable)
        {
            // Normal.
            CBoxDDNorReg.IsEnabled = inputEnable;
            CBoxDDNorMisT1.IsEnabled = inputEnable;
            CBoxDDNorMisT2.IsEnabled = inputEnable;
            CBoxDDNorMisT3.IsEnabled = inputEnable;
            TBoxDDNorMisV1.IsEnabled = inputEnable;
            TBoxDDNorMisV2.IsEnabled = inputEnable;
            TBoxDDNorMisV3.IsEnabled = inputEnable;
            CBoxDDNorObjT1.IsEnabled = inputEnable;
            CBoxDDNorObjT2.IsEnabled = inputEnable;
            CBoxDDNorObjT3.IsEnabled = inputEnable;
            TBoxDDNorObjV1.IsEnabled = inputEnable;
            TBoxDDNorObjV2.IsEnabled = inputEnable;
            TBoxDDNorObjV3.IsEnabled = inputEnable;
            CBoxDDNorWar1.IsEnabled = inputEnable;
            CBoxDDNorWar2.IsEnabled = inputEnable;
            CBoxDDNorWar3.IsEnabled = inputEnable;
            CBoxDDNorAno1.IsEnabled = inputEnable;
            CBoxDDNorAno2.IsEnabled = inputEnable;
            CBoxDDNorAno3.IsEnabled = inputEnable;
            // Elite.
            CBoxDDEliReg.IsEnabled = inputEnable;
            CBoxDDEliMisT1.IsEnabled = inputEnable;
            CBoxDDEliMisT2.IsEnabled = inputEnable;
            CBoxDDEliMisT3.IsEnabled = inputEnable;
            TBoxDDEliMisV1.IsEnabled = inputEnable;
            TBoxDDEliMisV2.IsEnabled = inputEnable;
            TBoxDDEliMisV3.IsEnabled = inputEnable;
            CBoxDDEliObjT1.IsEnabled = inputEnable;
            CBoxDDEliObjT2.IsEnabled = inputEnable;
            CBoxDDEliObjT3.IsEnabled = inputEnable;
            TBoxDDEliObjV1.IsEnabled = inputEnable;
            TBoxDDEliObjV2.IsEnabled = inputEnable;
            TBoxDDEliObjV3.IsEnabled = inputEnable;
            CBoxDDEliWar1.IsEnabled = inputEnable;
            CBoxDDEliWar2.IsEnabled = inputEnable;
            CBoxDDEliWar3.IsEnabled = inputEnable;
            CBoxDDEliAno1.IsEnabled = inputEnable;
            CBoxDDEliAno2.IsEnabled = inputEnable;
            CBoxDDEliAno3.IsEnabled = inputEnable;
        }
        public void BoxDivsInfo_Set(DataDive inputDeepDive)
        {
            // Normal.
            CBoxDDNorReg.Text = inputDeepDive.Normal.Region;
            CBoxDDNorMisT1.Text = inputDeepDive.Normal.Missions[0].Type;
            CBoxDDNorMisT2.Text = inputDeepDive.Normal.Missions[1].Type;
            CBoxDDNorMisT3.Text = inputDeepDive.Normal.Missions[2].Type;
            TBoxDDNorMisV1.Text = inputDeepDive.Normal.Missions[0].Value;
            TBoxDDNorMisV2.Text = inputDeepDive.Normal.Missions[1].Value;
            TBoxDDNorMisV3.Text = inputDeepDive.Normal.Missions[2].Value;
            CBoxDDNorObjT1.Text = inputDeepDive.Normal.Objectives[0].Type;
            CBoxDDNorObjT2.Text = inputDeepDive.Normal.Objectives[1].Type;
            CBoxDDNorObjT3.Text = inputDeepDive.Normal.Objectives[2].Type;
            TBoxDDNorObjV1.Text = inputDeepDive.Normal.Objectives[0].Value;
            TBoxDDNorObjV2.Text = inputDeepDive.Normal.Objectives[1].Value;
            TBoxDDNorObjV3.Text = inputDeepDive.Normal.Objectives[2].Value;
            CBoxDDNorWar1.Text = inputDeepDive.Normal.Warnings[0];
            CBoxDDNorWar2.Text = inputDeepDive.Normal.Warnings[1];
            CBoxDDNorWar3.Text = inputDeepDive.Normal.Warnings[2];
            CBoxDDNorAno1.Text = inputDeepDive.Normal.Anomalies[0];
            CBoxDDNorAno2.Text = inputDeepDive.Normal.Anomalies[1];
            CBoxDDNorAno3.Text = inputDeepDive.Normal.Anomalies[2];
            // Elite.
            CBoxDDEliReg.Text = inputDeepDive.Elite.Region;
            CBoxDDEliMisT1.Text = inputDeepDive.Elite.Missions[0].Type;
            CBoxDDEliMisT2.Text = inputDeepDive.Elite.Missions[1].Type;
            CBoxDDEliMisT3.Text = inputDeepDive.Elite.Missions[2].Type;
            TBoxDDEliMisV1.Text = inputDeepDive.Elite.Missions[0].Value;
            TBoxDDEliMisV2.Text = inputDeepDive.Elite.Missions[1].Value;
            TBoxDDEliMisV3.Text = inputDeepDive.Elite.Missions[2].Value;
            CBoxDDEliObjT1.Text = inputDeepDive.Elite.Objectives[0].Type;
            CBoxDDEliObjT2.Text = inputDeepDive.Elite.Objectives[1].Type;
            CBoxDDEliObjT3.Text = inputDeepDive.Elite.Objectives[2].Type;
            TBoxDDEliObjV1.Text = inputDeepDive.Elite.Objectives[0].Value;
            TBoxDDEliObjV2.Text = inputDeepDive.Elite.Objectives[1].Value;
            TBoxDDEliObjV3.Text = inputDeepDive.Elite.Objectives[2].Value;
            CBoxDDEliWar1.Text = inputDeepDive.Elite.Warnings[0];
            CBoxDDEliWar2.Text = inputDeepDive.Elite.Warnings[1];
            CBoxDDEliWar3.Text = inputDeepDive.Elite.Warnings[2];
            CBoxDDEliAno1.Text = inputDeepDive.Elite.Anomalies[0];
            CBoxDDEliAno2.Text = inputDeepDive.Elite.Anomalies[1];
            CBoxDDEliAno3.Text = inputDeepDive.Elite.Anomalies[2];
        }
        public void TmrDivsSearch_Start()
        {
            TmrDivsSearch.Start();
        }
        public void TmrDivsSearch_Stop()
        {
            TmrDivsSearch.Stop();
        }
        public void TmrDivsSearch_Update()
        {
            TmrDivsSearch_Stop();
            TmrDivsSearch_Start();
        }
        public void VListDivs_Fill()
        {
            SourceVlistDives.Clear();
            List<DataDive> dataDives = Data_Dives_Get_Dives(SettingsCurrent.Version.SelectedId, SettingsCurrent.Dive.Search);
            dataDives = Data_Dives_Sort_Dives(dataDives);
            for (int i = 0; i < dataDives.Count; i++)
            {
                // Create item.
                SrcVListDive srcVListDive = new SrcVListDive();
                srcVListDive.Number = dataDives[i].Number;
                srcVListDive.Date = dataDives[i].Date;
                srcVListDive.NameNormal = dataDives[i].Normal.Name;
                srcVListDive.NameElite = dataDives[i].Elite.Name;
                srcVListDive.BrushBack = dataDives[i].BrushBack;
                if (dataDives[i].EventNumber != "")
                {
                    srcVListDive.Event = "★";
                }
                // Add item.
                SourceVlistDives.Add(srcVListDive);
            }
            VListDivs.SelectedIndex = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Dive.SelectedNumber, dataDives);
        }
        public void VListDivs_SelectId(int inIdDive, bool inSkip = false)
        {
            bool change = true;
            int idDiveOld = SettingsCurrent.Dive.SelectedId;
            //
            if (inSkip == false)
            {
            }
            //
            if (change == true)
            {
                Settings_Save_Dive_SelectedId(inIdDive);
                //
                DataDive dataDive = Data_Dives_Get_Dive(SettingsCurrent.Version.SelectedId, inIdDive);
                if (inIdDive >= 0)
                {
                    TBoxDivsSeed.IsEnabled = false;
                    TBoxDivsSeed.Text = dataDive.Seed.ToString();
                    BtnDivsSeed.IsEnabled = false;
                    BoxDivsInfo_Set(dataDive);
                }
                else
                {
                    TBoxDivsSeed.IsEnabled = true;
                    TBoxDivsSeed.Text = SettingsCurrent.Dive.Seed.ToString();
                    BtnDivsSeed.IsEnabled = true;
                    BoxDivsInfo_Set(dataDive);
                }
                //
                if (idDiveOld < 0 && inIdDive >= 0 || idDiveOld >= 0 && inIdDive < 0 || idDiveOld >= 0 && inIdDive >= 0)
                {
                    int idEvent = -1;
                    if (inIdDive >= 0)
                    {
                        idEvent = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, Data.Versions[SettingsCurrent.Version.SelectedId].Dives[inIdDive].EventNumber);
                    }
                    if (SettingsCurrent.Event.SelectedId != idEvent)
                    {
                        VListEvts_SelectId(idEvent, true);
                        VListEvts_Fill();
                    }
                }
            }
        }
        // Tab Events.
        public void TabEvents_Load()
        {
            //
            CBoxEvtsFreeBeers.IsChecked = SettingsCurrent.Event.FreeBeers;
            //
            CBoxEvtsLost.IsChecked = SettingsCurrent.Event.LostEvents;
            //
            TmrEvtsSearch.Tick += TmrEvtsSearch_Tick;
            TmrEvtsSearch.Interval = TimeSpan.FromSeconds(0.5);
            //
            VListEvts.ItemsSource = SourceVlistEvents;
            VListEvts_SelectId(SettingsCurrent.Event.SelectedId, true);
            VListEvts_Fill();
        }
        public void TmrEvtsSearch_Start()
        {
            TmrEvtsSearch.Start();
        }
        public void TmrEvtsSearch_Stop()
        {
            TmrEvtsSearch.Stop();
        }
        public void TmrEvtsSearch_Update()
        {
            TmrEvtsSearch_Stop();
            TmrEvtsSearch_Start();
        }
        public void VListEvts_Fill()
        {
            SourceVlistEvents.Clear();
            List<DataEvent> dataEvents = Data_Events_Get_Events(SettingsCurrent.Version.SelectedId, SettingsCurrent.Event.Search);
            dataEvents = Data_Events_Sort_Events(dataEvents);
            for (int i = 0; i < dataEvents.Count; i++)
            {
                // Create item.
                SrcVListEvent srcVListEvent = new SrcVListEvent();
                srcVListEvent.Number = dataEvents[i].Number;
                srcVListEvent.Name = dataEvents[i].Name;
                srcVListEvent.Date = dataEvents[i].Date;
                srcVListEvent.BrushBack = dataEvents[i].BrushBack;
                // Add item.
                SourceVlistEvents.Add(srcVListEvent);
            }
            VListEvts.SelectedIndex = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, SettingsCurrent.Event.SelectedNumber, dataEvents);
        }
        public void VListEvts_SelectId(int inId, bool inSkip = false)
        {
            bool change = true;
            //
            if (inSkip == false)
            {
                int idDive = SettingsCurrent.Dive.SelectedId;
                if (idDive >= 0 && Data.Versions[SettingsCurrent.Version.SelectedId].Dives[idDive].EventNumber == SettingsCurrent.Event.SelectedNumber)
                {
                    string message = "Changing Event may affect Deep Dive creation. Do you still want to continue?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, this.Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        change = false;
                        // Update list, because item was selected, but it must not be.
                        VListEvts_Fill();
                    }
                }
            }
            if (change == true)
            {
                Settings_Save_Event_SelectedId(inId);
                //
                DataEvent dataEvent = Data_Events_Get_Event(SettingsCurrent.Version.SelectedId, inId);
                if (inId >= 0)
                {
                    TBoxEvtsCommand.IsEnabled = false;
                    TBoxEvtsCommand.Text = dataEvent.Command;
                }
                else
                {
                    TBoxEvtsCommand.IsEnabled = true;
                    TBoxEvtsCommand.Text = SettingsCurrent.Event.Command;
                }
            }
        }
        // Tab Assignments.
        public void TabAssignments_Load()
        {
            //
            TBoxAssSeed.Text = SettingsCurrent.Assignment.Seed.ToString();
        }
        // Tab Settings.
        public void TabSettings_Load()
        {
            //
            TBoxSetsPosX.Text = SettingsCurrent.Common.PosX.ToString();
            TBoxSetsPosY.Text = SettingsCurrent.Common.PosY.ToString();
            Window_Set_Position(SettingsCurrent.Common.PosX, SettingsCurrent.Common.PosY);
            //
            TBoxSetsSizeX.Text = SettingsCurrent.Common.SizeX.ToString();
            TBoxSetsSizeY.Text = SettingsCurrent.Common.SizeY.ToString();
            Window_Set_Size(SettingsCurrent.Common.SizeX, SettingsCurrent.Common.SizeY);
        }
        // Tab Help.
        public void TabHelp_Load()
        {
            string pathFile = Path.Combine(PathFoldDataLanguages, @"English\Help.txt");
            if (File.Exists(pathFile))
            {
                TextRange textRange = new TextRange(RTBoxHelp.Document.ContentStart, RTBoxHelp.Document.ContentEnd);
                FileStream fileStream = new FileStream(pathFile, FileMode.OpenOrCreate);
                textRange.Load(fileStream, DataFormats.Text);
                fileStream.Close();
            }
        }
        #endregion
        #region Triggers
        // Window.
        private void Window_Initialized(object sender, EventArgs e)
        {
            // In case of shutting down the app.exe with Task Manager and leaving server processes running.
            Server_Stop();
            //
            Version_Load();
            Data_Load_All();
            Settings_Load_All();
            //
            // Load window properties.
            Window_Load();
            // Load tab properties.
            TabServices_Load();
            TabVersions_Load();
            TabMods_Load();
            TabDives_Load();
            TabEvents_Load();
            TabAssignments_Load();
            TabSettings_Load();
            TabHelp_Load();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Server_Stop();
            if (SettingsCurrent.Services.ChangeRedirects == true)
            {
                Server_Redirects_Remove();
            }
            if (SettingsCurrent.Services.ChangeCertificates == true)
            {
                Server_Certificates_Remove();
            }
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // When window is minimized, it's position will be -32000, -32000.
            if (this.WindowState != WindowState.Minimized)
            {
                TmrWindowPos_Update();
            }
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            Color color = Color.FromArgb(255, 224, 224, 224);
            RectOutlineLeft.Fill = new SolidColorBrush(color);
            RectOutlineRight.Fill = new SolidColorBrush(color);
            RectOutlineTop.Fill = new SolidColorBrush(color);
            RectOutlineBottom.Fill = new SolidColorBrush(color);
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            Color color = Color.FromArgb(0, 0, 0, 0);
            RectOutlineLeft.Fill = new SolidColorBrush(color);
            RectOutlineRight.Fill = new SolidColorBrush(color);
            RectOutlineTop.Fill = new SolidColorBrush(color);
            RectOutlineBottom.Fill = new SolidColorBrush(color);
        }
        private void TmrWindowPos_Tick(object sender, EventArgs e)
        {
            double posX = this.Left;
            double posY = this.Top;
            //
            Settings_Save_Common_PosX(posX);
            Settings_Save_Common_PosY(posY);
            //
            TBoxSetsPosX.Text = posX.ToString();
            TBoxSetsPosY.Text = posY.ToString();
            TmrWindowPos_Stop();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // When window is minimized, it's size will be 0, 0.
            // When window is maximized, it's size will be +14, +14.
            if (this.WindowState == WindowState.Normal)
            {
                TmrWindowSize_Update();
            }
        }
        private void TmrWindowSize_Tick(object sender, EventArgs e)
        {
            double sizeX = this.ActualWidth;
            double sizeY = this.ActualHeight;
            //
            Settings_Save_Common_SizeX(sizeX);
            Settings_Save_Common_SizeY(sizeY);
            //
            TBoxSetsSizeX.Text = sizeX.ToString();
            TBoxSetsSizeY.Text = sizeY.ToString();
            //
            TmrWindowSize_Stop();
        }
        // App.
        private void BtnAppRldData_Click(object sender, RoutedEventArgs e)
        {
            Data_Load_All();
            Settings_Load_Version_PathComplex();
            Settings_Load_Mods_EnabledMltVersion();
            //
            int idVersion = Data_Versions_Get_Id(SettingsCurrent.Version.SelectedNumber);
            VListVers_SelectId(idVersion);
            VListVers_Fill();
        }
        // Tab Services.
        private void TBoxSvcsRedsIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ip = TBoxSvcsRedsIP.Text;
            Settings_Save_Services_IP(ip);
        }
        private void BtnSvcsRedsAdd_Click(object sender, RoutedEventArgs e)
        {
            // Remove redirects to avoid duplication and remove old version.
            Server_Redirects_Remove();
            // Add redirects.
            Server_Redirects_Add();
            // Update info panel.
            bool redirectsAdded = Server_Redirects_Check();
            CBoxSvcsRedsAdded_Enable(redirectsAdded);
            // Update timer, in case changes were not picked up instantly.
            if (redirectsAdded == false)
            {
                TmrRedsAdded_Update();
            }
        }
        private void BtnSvcsRedsRemove_Click(object sender, RoutedEventArgs e)
        {
            // Remove redirects.
            Server_Redirects_Remove();
            // Update info panel.
            bool redirectsAdded = Server_Redirects_Check();
            CBoxSvcsRedsAdded_Enable(redirectsAdded);
            // Update timer, in case changes were not picked up instantly.
            if (redirectsAdded == false)
            {
                TmrRedsAdded_Update();
            }
        }
        private void TmrSvcsStatusRedirects_Tick(object sender, EventArgs e)
        {
            CBoxSvcsRedsAdded_Enable(Server_Redirects_Check());
            TmrRedsAdded_Stop();
        }
        private void BtnSvcsRedsOpen_Click(object sender, RoutedEventArgs e)
        {
            Server_Redirects_OpenFolder();
        }
        private void CBoxSvcsRedsChange_Click(object sender, RoutedEventArgs e)
        {
            bool changeRedirects = CBoxSvcsRedsChange.IsChecked.Value;
            Settings_Save_Services_ChangeRedirects(changeRedirects);
        }
        private void BtnSvcsCertsAdd_Click(object sender, RoutedEventArgs e)
        {
            // Remove old version, no need to remove for dublication, because certificates can't be duplicated.
            Server_Certificates_Remove();
            // Add certificates.
            Server_Certificates_Add();
            // Update info panel.
            bool certificatesAdded = Server_Certificates_Check();
            CBoxSvcsCertsAdded_Enable(certificatesAdded);
            // Update timer, in case changes were not picked up instantly.
            if (certificatesAdded == false)
            {
                TmrCertsAdded_Update();
            }
        }
        private void BtnSvcsCertsRemove_Click(object sender, RoutedEventArgs e)
        {
            // Remove certificates.
            Server_Certificates_Remove();
            // Update info panel.
            bool certificatesAdded = Server_Certificates_Check();
            CBoxSvcsCertsAdded_Enable(certificatesAdded);
            // Update timer, in case changes were not picked up instantly.
            if (certificatesAdded == false)
            {
                TmrCertsAdded_Update();
            }
        }
        private void TmrSvcsStatusCertificates_Tick(object sender, EventArgs e)
        {
            CBoxSvcsCertsAdded_Enable(Server_Certificates_Check());
            TmrCertsAdded_Stop();
        }
        private void BtnSvcsCertsOpen_Click(object sender, RoutedEventArgs e)
        {
            Server_Certificates_OpenFolder();
        }
        private void CBoxSvcsCertsChange_Click(object sender, RoutedEventArgs e)
        {
            bool changeCerts = CBoxSvcsCertsChange.IsChecked.Value;
            Settings_Save_Services_ChangeCertificates(changeCerts);
        }
        private void BtnSvcsServStart_Click(object sender, RoutedEventArgs e)
        {
            Server_Stop();
            Server_Start();
            // Update info panel.
            bool serverRunning = Server_Check();
            CBoxSvcsServRunning_Enable(serverRunning);
            // Update timer, in case changes were not picked up instantly.
            if (serverRunning == false)
            {
                TmrServRunning_Update();
            }
        }
        private void BtnSvcsServStop_Click(object sender, RoutedEventArgs e)
        {
            Server_Stop();
            // Update info panel.
            bool serverRunning = Server_Check();
            CBoxSvcsServRunning_Enable(serverRunning);
            // Update timer, in case changes were not picked up instantly.
            if (serverRunning == false)
            {
                TmrServRunning_Update();
            }
        }
        private void TmrSvcsStatusServer_Tick(object sender, EventArgs e)
        {
            CBoxSvcsServRunning_Enable(Server_Check());
            TmrServRunning_Stop();
        }
        private void CBoxSvcsServChange_Click(object sender, RoutedEventArgs e)
        {
            bool startOnLaunch = CBoxSvcsServChange.IsChecked.Value;
            Settings_Save_Services_StartServer(startOnLaunch);
        }
        private void TmrSvcsStatus_Tick(object sender, EventArgs e)
        {
            CBoxSvcsRedsAdded_Enable(Server_Redirects_Check());
            CBoxSvcsCertsAdded_Enable(Server_Certificates_Check());
            CBoxSvcsServRunning_Enable(Server_Check());
        }
        // Tab Versions.
        private void TBoxVersPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            int idVersion = SettingsCurrent.Version.SelectedId;
            string path = TBoxVersPath.Text;
            Settings_Save_Version_Path(idVersion, path);
            //
            Settings_Load_Mods_EnabledSngVersion(idVersion); // Recheck maybe no need.
            //
            VListMods_Fill();
        }
        private void BtnVersBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                TBoxVersPath.Text = dialog.FileName;
            }
        }
        private void BtnVersLaunch_Click(object sender, RoutedEventArgs e)
        {
            string path;
            if (SettingsCurrent.Version.SelectedId >= 0)
            {
                path = Data.Versions[SettingsCurrent.Version.SelectedId].Path;
            }
            else
            {
                path = SettingsCurrent.Version.Path;
            }
            //
            if (path != "")
            {
                bool? check = Game_Check_Path(path);
                if (check != null)
                {
                    if (check == true)
                    {
                        if (SettingsCurrent.Version.PlayerId != null)
                        {
                            GSE_Launch(SettingsCurrent.Version.SelectedId);
                        }
                    }
                    else
                    {
                        // File is not FSD-Win64-Shipping.exe.
                        string title = this.Title;
                        string message = "File `" + path + "` is not correct executable file.";
                        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // File doesn't exist.
                    string title = this.Title;
                    string message = "Path `" + path + "` doesn't exist.";
                    System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // User didn't define path.
                string title = this.Title;
                string message = "Executable path is not defined.";
                System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CBoxVerBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // `.Text` will return previous value.
            string branch = (sender as ComboBox).SelectedItem as string;
            Settings_Save_Version_Branch(branch);
        }
        private void TBoxVersPlrId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string playerId = TBoxVersPlrId.Text;
            Settings_Save_Version_PlayerId(playerId);
        }
        private void TBoxVersPlrName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string playerName = TBoxVersPlrName.Text;
            Settings_Save_Version_PlayerName(playerName);
        }
        private void TBoxVersPlrName_LostFocus(object sender, RoutedEventArgs e)
        {
            string setting = TBoxVersPlrName.Text;
            if (setting == "")
            {
                TBoxVersPlrName.Text = SettingsDefault.Version.PlayerName;
            }
        }
        private void TBoxVersCmdLnch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string command = TBoxVersCmdLnch.Text;
            Settings_Save_Version_Command(command);
        }
        private void BtnVersCmdDepotCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("download_depot 548430 548431 " + Data.Versions[SettingsCurrent.Version.SelectedId].Manifest);
        }
        private void BtnVersSteamOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Steam_CheckRunning() == true)
            {
                Steam_OpenConsole();
            }
            else
            {
                string text = "Steam is not launched.";
                System.Windows.MessageBox.Show(text, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TBoxVersSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TmrVersSearch_Update();
        }
        private void TmrVersSearch_Tick(object sender, EventArgs e)
        {
            // Check if text changed, to minimize actions.
            string text = TBoxVersSearch.Text;
            if (text != SettingsCurrent.Version.Search)
            {
                Settings_Save_Version_Search(text);
                //
                VListVers_Fill();
                TmrVersSearch_Stop();
            }
        }
        private void VListVers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VListVers.SelectedIndex >= 0)
            {
                int idVersion = Data_Versions_Get_Id(SourceVlistVersions[VListVers.SelectedIndex].Number);
                if (SettingsCurrent.Version.SelectedId != idVersion)
                {
                    VListVers_SelectId(idVersion);
                }
            }
        }
        private void BtnVersDeselect_Click(object sender, RoutedEventArgs e)
        {
            VListVers_SelectId(-1);
            VListVers_Fill();
        }
        // Tab Mods.
        private void BtnModsBackup_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(PathFileModIOState) == true)
            {
                string text = File.ReadAllText(PathFileModIOState);
                JObject jObject = JObject.Parse(text);
                if (jObject["Mods"] != null && jObject["Mods"].Count<JToken>() > 0)
                {
                    for (int i = 0; i < jObject["Mods"].Count<JToken>(); i++)
                    {
                        JToken jTPathFoldMod = jObject["Mods"][i]["PathOnDisk"];
                        if (jTPathFoldMod != null)
                        {
                            string pathFoldMod = jTPathFoldMod.ToString();
                            if (Directory.Exists(pathFoldMod) == true)
                            {
                                // Notice that search option is `AllDirectories`, because mod file can be inside the folder.
                                string[] pathFilesMod = Directory.GetFiles(pathFoldMod, "*.pak", SearchOption.AllDirectories);
                                // Folder should have only one file normally.
                                if (pathFilesMod.Length == 1)
                                {
                                    // Number.
                                    JToken jTNumberMod = jObject["Mods"][i]["ID"];
                                    if (jTNumberMod != null)
                                    {
                                        string numberMod = jTNumberMod.ToString();
                                        // Name.
                                        JToken jTNameMod = jObject["Mods"][i]["Profile"]["name"];
                                        if (jTNameMod != null)
                                        {
                                            string nameMod = jTNameMod.ToString();
                                            // Time.
                                            JToken jTTimeMod = jObject["Mods"][i]["Profile"]["date_updated"];
                                            if (jTTimeMod != null)
                                            {
                                                int timeMod = 0;
                                                if (int.TryParse(jTTimeMod.ToString(), out timeMod) == true)
                                                {
                                                    // Decription.
                                                    string descriptionMod = "";
                                                    JToken jTDecriptionMod = jObject["Mods"][i]["Profile"]["description_plaintext"];
                                                    if (jTDecriptionMod != null)
                                                    {
                                                        descriptionMod = String_FormatTo_Literal(jTDecriptionMod.ToString());
                                                    }
                                                    //
                                                    bool skip = false;
                                                    int idModOld = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, numberMod);
                                                    if (idModOld >= 0 && Data.Versions[SettingsCurrent.Version.SelectedId].Mods[idModOld].Time >= timeMod)
                                                    {
                                                        skip = true;
                                                    }
                                                    //
                                                    if (skip == false)
                                                    {
                                                        string pathFoldModNew = Path.Combine(PathFoldSavesMods, SettingsCurrent.Version.SelectedNumber, numberMod);
                                                        Directory.CreateDirectory(pathFoldModNew);
                                                        File.Copy(pathFilesMod[0], Path.Combine(pathFoldModNew, "File.pak"));
                                                        string PathFileInfo = Path.Combine(pathFoldModNew, "Info.ini");
                                                        FileINI fileINI = new FileINI(PathFileInfo);
                                                        fileINI.WriteKey("Name", "Mod", nameMod);
                                                        fileINI.WriteKey("Time", "Mod", timeMod.ToString());
                                                        fileINI.WriteKey("Description", "Mod", descriptionMod);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void TBoxModsSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TmrModsSearch_Update();
        }
        private void TmrModsSearch_Tick(global::System.Object sender, global::System.EventArgs e)
        {
            // Check if text changed, to minimize actions.
            string text = TBoxModsSearch.Text;
            if (text != SettingsCurrent.Mod.Search)
            {
                Settings_Save_Mod_Search(text);
                //
                VListMods_Fill();
                TmrModsSearch_Stop();
            }
        }
        private void VListMods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VListMods.SelectedIndex >= 0)
            {
                int idMod = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, SourceVlistMods[VListMods.SelectedIndex].Number);
                if (SettingsCurrent.Mod.SelectedId != idMod)
                {
                    VListMods_SelectId(idMod);
                }
            }
        }
        private void CBoxModsModIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            int idVersion = SettingsCurrent.Version.SelectedId;
            if (Data.Versions[idVersion].Path != "" && File.Exists(Data.Versions[idVersion].Path))
            {
                int idMod = SettingsCurrent.Mod.SelectedId;
                bool modIsEnabled = !Data.Versions[idVersion].Mods[idMod].IsEnabled;
                Settings_Save_Mod_ModIsEnabled(idVersion, idMod, modIsEnabled);
            }
            else
            {
                CheckBox checkBox = sender as CheckBox;
                checkBox.IsChecked = false;
            }
        }
        private void BtnModsDeselect_Click(object sender, RoutedEventArgs e)
        {
            VListMods_SelectId(-1);
            VListMods_Fill();
        }
        // Tab Dives.
        private void TBoxDivsSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxDivsSeed.Text, TBoxDivsSeed.SelectionStart, out string str, out int pos);
            TBoxDivsSeed.Text = str;
            TBoxDivsSeed.SelectionStart = pos;
            // Try to parse, if can't parse don't save, because string can only be empty, and emty string will be reset to default on focus loss.
            if (uint.TryParse(TBoxDivsSeed.Text, out uint seed) == true)
            {
                if (SettingsCurrent.Dive.SelectedId >= 0)
                {
                    File_Apache24_Write_Dives(seed);
                }
                else
                {
                    Settings_Save_Dive_Seed(seed);
                }
            }
        }
        private void TBoxDivsSeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxDivsSeed.Text == "")
            {
                TBoxDivsSeed.Text = SettingsCurrent.Dive.Seed.ToString();
            }
        }
        private void BtnDivsSeed_Click(object sender, RoutedEventArgs e)
        {
            uint seed = Number_Generate_Seed();
            TBoxDivsSeed.Text = seed.ToString();
        }
        private void CBoxDivsLost_Click(object sender, RoutedEventArgs e)
        {
            bool setting = CBoxDivsLost.IsChecked.Value;
            Settings_Save_Dive_LostDives(setting);
            VListDivs_Fill();
        }
        private void TBoxDivsSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TmrDivsSearch_Update();
        }
        private void TmrDivsSearch_Tick(global::System.Object sender, global::System.EventArgs e)
        {
            // Check if text changed, to minimize actions, when search text was added, then removed.
            string text = TBoxDivsSearch.Text;
            if (text != SettingsCurrent.Dive.Search)
            {
                Settings_Save_Dive_Search(text);
                //
                VListDivs_Fill();
                TmrDivsSearch_Stop();
            }
        }
        private void VListDivs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VListDivs.SelectedIndex >= 0)
            {
                int idDive = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, SourceVlistDives[VListDivs.SelectedIndex].Number);
                if (SettingsCurrent.Dive.SelectedId != idDive)
                {
                    VListDivs_SelectId(idDive);
                }
            }
        }
        private void BtnDivsDeselect_Click(object sender, RoutedEventArgs e)
        {
            VListDivs_SelectId(-1);
            VListDivs_Fill();
        }
        // Tab Events.
        private void TBoxEvtsCommand_TextChanged(object sender, TextChangedEventArgs e)
        {
            string command = TBoxEvtsCommand.Text;
            if (SettingsCurrent.Event.SelectedId >= 0)
            {
                File_Apache24_Write_Events(command);
            }
            else
            {
                Settings_Save_Event_Command(command);
            }
        }
        private void CBoxEvtsFreeBeers_Click(object sender, RoutedEventArgs e)
        {
            bool freeBeers = CBoxEvtsFreeBeers.IsChecked.Value;
            Settings_Save_Event_FreeBeers(freeBeers);
        }
        private void CBoxEvtsLost_Click(object sender, RoutedEventArgs e)
        {
            bool setting = CBoxEvtsLost.IsChecked.Value;
            Settings_Save_Event_LostEvents(setting);
            VListEvts_Fill();
        }
        private void TBoxEvtsSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TmrEvtsSearch_Update();
        }
        private void TmrEvtsSearch_Tick(global::System.Object sender, global::System.EventArgs e)
        {
            // Check if text changed, to minimize actions, when search text was added, then removed.
            string text = TBoxEvtsSearch.Text;
            if (text != SettingsCurrent.Event.Search)
            {
                Settings_Save_Event_Search(text);
                //
                VListEvts_Fill();
                TmrEvtsSearch_Stop();
            }
        }
        private void VListEvts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VListEvts.SelectedIndex >= 0)
            {
                int idEvent = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, SourceVlistEvents[VListEvts.SelectedIndex].Number);
                if (SettingsCurrent.Event.SelectedId != idEvent)
                {
                    VListEvts_SelectId(idEvent);
                }
            }
        }
        private void BtnEvtsDeselect_Click(object sender, RoutedEventArgs e)
        {
            VListEvts_SelectId(-1);
            VListEvts_Fill();
        }
        // Tab Assignments.
        private void TBoxAssSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxAssSeed.Text, TBoxAssSeed.SelectionStart, out string text, out int pos);
            TBoxAssSeed.Text = text;
            TBoxAssSeed.SelectionStart = pos;
            // Try to parse, if can't parse don't save, because string can only be empty, and emty string will be reset to default on focus loss.
            if (uint.TryParse(TBoxAssSeed.Text, out uint seed) == true && seed != SettingsCurrent.Assignment.Seed)
            {
                Settings_Save_Assignment_Seed(seed);
            }
        }
        private void TBoxAssSeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxAssSeed.Text == "")
            {
                TBoxAssSeed.Text = SettingsCurrent.Assignment.Seed.ToString();
            }
        }
        private void BtnAssSeed_Click(object sender, RoutedEventArgs e)
        {
            uint seed = Number_Generate_Seed();
            TBoxAssSeed.Text = seed.ToString();
        }
        // Tab Settings.
        private void TBoxSetsPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_Int32(TBoxSetsPosX.Text, TBoxSetsPosX.SelectionStart, out string text, out int pos);
            TBoxSetsPosX.Text = text;
            TBoxSetsPosX.SelectionStart = pos;
        }
        private void TBoxSetsPosX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSetsPosX.Text == "" || TBoxSetsPosX.Text == "-")
            {
                TBoxSetsPosX.Text = SettingsCurrent.Common.PosX.ToString();
            }
        }
        private void TBoxSetsPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_Int32(TBoxSetsPosY.Text, TBoxSetsPosY.SelectionStart, out string text, out int pos);
            TBoxSetsPosY.Text = text;
            TBoxSetsPosY.SelectionStart = pos;
        }
        private void TBoxSetsPosY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSetsPosY.Text == "" || TBoxSetsPosY.Text == "-")
            {
                TBoxSetsPosY.Text = SettingsCurrent.Common.PosY.ToString();
            }
        }
        private void BtnSetsPos_Click(object sender, RoutedEventArgs e)
        {
            double posX = double.Parse(TBoxSetsPosX.Text);
            double posY = double.Parse(TBoxSetsPosY.Text);
            // Check to avoid double trigger.
            if (posX != SettingsCurrent.Common.PosX || posY != SettingsCurrent.Common.PosY)
            {
                if (posX >= 0 && posX <= SystemParameters.VirtualScreenWidth && posY >= 0 && posY <= SystemParameters.VirtualScreenHeight)
                {
                    Settings_Save_Common_PosX(posX);
                    Settings_Save_Common_PosY(posY);
                    //
                    Window_Set_Position(posX, posY);
                }
                else
                {
                    string title = this.Title;
                    string message = "Defined position is off the screen. Do you still want to apply it?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Settings_Save_Common_PosX(posX);
                        Settings_Save_Common_PosY(posY);
                        //
                        Window_Set_Position(posX, posY);
                    }
                    else
                    {
                        TBoxSetsPosX.Text = SettingsCurrent.Common.PosX.ToString();
                        TBoxSetsPosY.Text = SettingsCurrent.Common.PosY.ToString();
                    }
                }
            }
        }
        private void TBoxSetsSizeX_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxSetsSizeX.Text, TBoxSetsSizeX.SelectionStart, out string text, out int pos);
            TBoxSetsSizeX.Text = text;
            TBoxSetsSizeX.SelectionStart = pos;
        }
        private void TBoxSetsSizeX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSetsSizeX.Text == "")
            {
                TBoxSetsSizeX.Text = SettingsCurrent.Common.SizeX.ToString();
            }
        }
        private void TBoxSetsSizeY_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxSetsSizeY.Text, TBoxSetsSizeY.SelectionStart, out string text, out int pos);
            TBoxSetsSizeY.Text = text;
            TBoxSetsSizeY.SelectionStart = pos;
        }
        private void TBoxSetsSizeY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSetsSizeY.Text == "")
            {
                TBoxSetsSizeY.Text = SettingsCurrent.Common.SizeY.ToString();
            }
        }
        private void BtnSetsSize_Click(object sender, RoutedEventArgs e)
        {
            double sizeX = double.Parse(TBoxSetsSizeX.Text);
            double sizeY = double.Parse(TBoxSetsSizeY.Text);
            // Check to avoid double trigger.
            if (sizeX != SettingsCurrent.Common.SizeX || sizeY != SettingsCurrent.Common.SizeY)
            {
                if (sizeX <= SystemParameters.VirtualScreenWidth && sizeY <= SystemParameters.VirtualScreenHeight)
                {
                    Settings_Save_Common_SizeX(sizeX);
                    Settings_Save_Common_SizeY(sizeY);
                    //
                    Window_Set_Size(sizeX, sizeY);
                }
                else
                {
                    string title = this.Title;
                    string message = "Defined size is bigger than the screen. Do you still want to apply it?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Settings_Save_Common_SizeX(sizeX);
                        Settings_Save_Common_SizeY(sizeY);
                        //
                        Window_Set_Size(sizeX, sizeY);
                    }
                    else
                    {
                        TBoxSetsSizeX.Text = SettingsCurrent.Common.SizeX.ToString();
                        TBoxSetsSizeY.Text = SettingsCurrent.Common.SizeY.ToString();
                    }
                }
            }
        }
        #endregion

        private void BtnDivsSave_Click(object sender, RoutedEventArgs e)
        {
            string pathFold = Path.Combine(PathFoldSavesDives, SettingsCurrent.Version.SelectedNumber);
            Directory.CreateDirectory(pathFold);
            string pathFile = Path.Combine(pathFold, SettingsCurrent.Dive.SelectedNumber + ".ini");
            FileINI fileINI = new FileINI(pathFile);
            // Common.
            fileINI.WriteKey("Date", "Dive", Data.Versions[SettingsCurrent.Version.SelectedId].Dives[SettingsCurrent.Dive.SelectedId].Date);
            fileINI.WriteKey("Seed", "Dive", TBoxDivsSeed.Text);
            fileINI.WriteKey("Event", "Dive", Data.Versions[SettingsCurrent.Version.SelectedId].Dives[SettingsCurrent.Dive.SelectedId].EventNumber);
            // Normal.
            fileINI.WriteKey("NormalName", "Dive", Data.Versions[SettingsCurrent.Version.SelectedId].Dives[SettingsCurrent.Dive.SelectedId].Normal.Name);
            fileINI.WriteKey("NormalRegion", "Dive", CBoxDDNorReg.Text);
            fileINI.WriteKey("NormalMissionType1", "Dive", CBoxDDNorMisT1.Text);
            fileINI.WriteKey("NormalMissionType2", "Dive", CBoxDDNorMisT2.Text);
            fileINI.WriteKey("NormalMissionType3", "Dive", CBoxDDNorMisT3.Text);
            fileINI.WriteKey("NormalMissionValue1", "Dive", TBoxDDNorMisV1.Text);
            fileINI.WriteKey("NormalMissionValue2", "Dive", TBoxDDNorMisV2.Text);
            fileINI.WriteKey("NormalMissionValue3", "Dive", TBoxDDNorMisV3.Text);
            fileINI.WriteKey("NormalObjectiveType1", "Dive", CBoxDDNorObjT1.Text);
            fileINI.WriteKey("NormalObjectiveType2", "Dive", CBoxDDNorObjT2.Text);
            fileINI.WriteKey("NormalObjectiveType3", "Dive", CBoxDDNorObjT3.Text);
            fileINI.WriteKey("NormalObjectiveValue1", "Dive", TBoxDDNorObjV1.Text);
            fileINI.WriteKey("NormalObjectiveValue2", "Dive", TBoxDDNorObjV2.Text);
            fileINI.WriteKey("NormalObjectiveValue3", "Dive", TBoxDDNorObjV3.Text);
            fileINI.WriteKey("NormalWarning1", "Dive", CBoxDDNorWar1.Text);
            fileINI.WriteKey("NormalWarning2", "Dive", CBoxDDNorWar2.Text);
            fileINI.WriteKey("NormalWarning3", "Dive", CBoxDDNorWar3.Text);
            fileINI.WriteKey("NormalAnomaly1", "Dive", CBoxDDNorAno1.Text);
            fileINI.WriteKey("NormalAnomaly2", "Dive", CBoxDDNorAno2.Text);
            fileINI.WriteKey("NormalAnomaly3", "Dive", CBoxDDNorAno3.Text);
            // Elite.
            fileINI.WriteKey("EliteName", "Dive", Data.Versions[SettingsCurrent.Version.SelectedId].Dives[SettingsCurrent.Dive.SelectedId].Elite.Name);
            fileINI.WriteKey("EliteRegion", "Dive", CBoxDDEliReg.Text);
            fileINI.WriteKey("EliteMissionType1", "Dive", CBoxDDEliMisT1.Text);
            fileINI.WriteKey("EliteMissionType2", "Dive", CBoxDDEliMisT2.Text);
            fileINI.WriteKey("EliteMissionType3", "Dive", CBoxDDEliMisT3.Text);
            fileINI.WriteKey("EliteMissionValue1", "Dive", TBoxDDEliMisV1.Text);
            fileINI.WriteKey("EliteMissionValue2", "Dive", TBoxDDEliMisV2.Text);
            fileINI.WriteKey("EliteMissionValue3", "Dive", TBoxDDEliMisV3.Text);
            fileINI.WriteKey("EliteObjectiveType1", "Dive", CBoxDDEliObjT1.Text);
            fileINI.WriteKey("EliteObjectiveType2", "Dive", CBoxDDEliObjT2.Text);
            fileINI.WriteKey("EliteObjectiveType3", "Dive", CBoxDDEliObjT3.Text);
            fileINI.WriteKey("EliteObjectiveValue1", "Dive", TBoxDDEliObjV1.Text);
            fileINI.WriteKey("EliteObjectiveValue2", "Dive", TBoxDDEliObjV2.Text);
            fileINI.WriteKey("EliteObjectiveValue3", "Dive", TBoxDDEliObjV3.Text);
            fileINI.WriteKey("EliteWarning1", "Dive", CBoxDDEliWar1.Text);
            fileINI.WriteKey("EliteWarning2", "Dive", CBoxDDEliWar2.Text);
            fileINI.WriteKey("EliteWarning3", "Dive", CBoxDDEliWar3.Text);
            fileINI.WriteKey("EliteAnomaly1", "Dive", CBoxDDEliAno1.Text);
            fileINI.WriteKey("EliteAnomaly2", "Dive", CBoxDDEliAno2.Text);
            fileINI.WriteKey("EliteAnomaly3", "Dive", CBoxDDEliAno3.Text);
        }
    }
}
