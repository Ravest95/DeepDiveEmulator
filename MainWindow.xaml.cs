using DeepDiveEmulator.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using Application = System.Windows.Forms.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using Clipboard = System.Windows.Clipboard;
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
                InitializeComponent();
            }
        }

        // Variables.
        #region Variables
        // Path folders.
        static string PathFoldApp = Application.StartupPath;
        static string PathFoldApache24 = Path.Combine(PathFoldApp, "Apache24");
        static string PathFoldAppDives = Path.Combine(PathFoldApp, "Data\\Dives");
        static string PathFoldAppEvents = Path.Combine(PathFoldApp, "Data\\Events");
        static string PathFoldAppMods = Path.Combine(PathFoldApp, "Data\\Mods");
        static string PathFoldAppVersions = Path.Combine(PathFoldApp, "Data\\Versions");
        static string PathFoldGSE = Path.Combine(PathFoldApp, "GoldbergSteamEmulator");
        static string PathFoldAppLanguages = Path.Combine(PathFoldApp, "Languages");
        static string PathFoldAppSaveVersions = Path.Combine(PathFoldApp, "Saves\\Versions");
        static string PathFoldAppSaveMods = Path.Combine(PathFoldApp, "Saves\\Mods");
        static string PathFoldApache24Certificates = Path.Combine(PathFoldApache24, "conf\\ssl.crt");
        static string PathFoldWindows = Environment.ExpandEnvironmentVariables("%SystemRoot%");
        static string PathFoldLocalAppData = Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%");
        static string PathFoldPublic = Environment.ExpandEnvironmentVariables("%PUBLIC%");
        static string PathFoldWindowsETC = Path.Combine(PathFoldWindows, "System32\\drivers\\etc");
        // Path files.
        static string PathFileAppSettings = Path.Combine(PathFoldApp, "DeepDiveEmulator.ini");
        static string PathFileAppAnomalies = Path.Combine(PathFoldApp, "Sources\\Anomalies.txt");
        static string PathFileAppMissions = Path.Combine(PathFoldApp, "Sources\\Missions.txt");
        static string PathFileAppObjectives = Path.Combine(PathFoldApp, "Sources\\Objectives.txt");
        static string PathFileAppRegions = Path.Combine(PathFoldApp, "Sources\\Regions.txt");
        static string PathFileAppWarnings = Path.Combine(PathFoldApp, "Sources\\Warnings.txt");
        static string PathFileHTTPDEXE = Path.Combine(PathFoldApache24, "bin\\httpd.exe");
        static string PathFileHTTPDConfig = Path.Combine(PathFoldApache24, "conf\\httpd.conf");
        static string PathFileApache24DeepDive1 = Path.Combine(PathFoldApache24, "htdocs\\drg.ghostship.dk\\events\\deepdive");
        static string PathFileApache24DeepDive2 = Path.Combine(PathFoldApache24, "htdocs\\services.ghostship.dk\\deepdive");
        static string PathFileApache24CGoalStateTime1 = Path.Combine(PathFoldApache24, "htdocs\\services.ghostship.dk\\cGoalStateTime");
        static string PathFileApache24Events1 = Path.Combine(PathFoldApache24, "htdocs\\drg.ghostship.dk\\events\\events");
        static string PathFileApache24Events2 = Path.Combine(PathFoldApache24, "htdocs\\services.ghostship.dk\\events");
        static string PathFileApache24Weekly1 = Path.Combine(PathFoldApache24, "htdocs\\drg.ghostship.dk\\events\\weekly");
        static string PathFileApache24Weekly2 = Path.Combine(PathFoldApache24, "htdocs\\services.ghostship.dk\\weekly");
        static string PathFileGSEColdClientLoader = Path.Combine(PathFoldGSE, "ColdClientLoader.ini");
        static string PathFileGSESteamClient_Loader = Path.Combine(PathFoldGSE, "steamclient_loader.exe");
        static string PathFileGSEUser_Steam_Id = Path.Combine(PathFoldGSE, "steam_settings\\settings\\user_steam_id.txt");
        static string PathFileGSEAccount_Name = Path.Combine(PathFoldGSE, "steam_settings\\settings\\account_name.txt");
        static string PathFileWindowsHosts = Path.Combine(PathFoldWindows, "System32\\drivers\\etc\\hosts");




        //
        static string PathFoldModIO = Path.Combine(PathFoldPublic, "mod.io");
        //
        static string PathFileModIOGlobalSettings = Path.Combine(PathFoldLocalAppData, "mod.io\\globalsettings.json");
        static string PathFileModIOUser = Path.Combine(PathFoldLocalAppData, "mod.io\\2475", WindowsIdentity.GetCurrent().User.ToString(), "user.json");
        static string PathFileModIOState = Path.Combine(PathFoldPublic, "mod.io\\2475\\metadata\\state.json");
        #endregion
        #region Timers
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
        #region Data
        public static Data Data = new Data();
        #endregion
        #region Sorces
        // CBox.
        public static SrcCBox SorceCBox = new SrcCBox();
        // Vlist.
        ObservableCollection<SrcVListVersion> SorceVlistVersions = new ObservableCollection<SrcVListVersion>();
        ObservableCollection<SrcVListMod> SorceVlistMods = new ObservableCollection<SrcVListMod>();
        ObservableCollection<SrcVListDive> SorceVlistDives = new ObservableCollection<SrcVListDive>();
        ObservableCollection<SrcVListEvent> SorceVlistEvents = new ObservableCollection<SrcVListEvent>();
        ObservableCollection<SrcVListEventItem> SorceVlistEventItems = new ObservableCollection<SrcVListEventItem>();
        #endregion
        #region Settings
        public static Settings SettingsDefault = new Settings();
        public static Settings SettingsCurrent = new Settings();
        #endregion

        // Functions.
        #region Source
        public void Source_Load_Anomalies()
        {
            // Clear list, because it may be reloaded later.
            SorceCBox.Anomalies = new List<string>();
            // Add epmty string.
            SorceCBox.Anomalies.Add("");
            // Load from file.
            if (File.Exists(PathFileAppAnomalies) == true)
            {
                string[] lines = File.ReadAllLines(PathFileAppAnomalies);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        SorceCBox.Anomalies.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppAnomalies));
                File.Create(PathFileAppAnomalies);
            }
        }
        public void Source_Load_Missions()
        {
            // Clear list, because it may be reloaded later.
            SorceCBox.Missions = new List<string>();
            // Add epmty string.
            SorceCBox.Missions.Add("");
            // Load from file.
            if (File.Exists(PathFileAppMissions) == true)
            {
                string[] lines = File.ReadAllLines(PathFileAppMissions);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        SorceCBox.Missions.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppMissions));
                File.Create(PathFileAppMissions);
            }
        }
        public void Source_Load_Regions()
        {
            // Clear list, because it may be reloaded later.
            SorceCBox.Regions = new List<string>();
            // Add epmty string.
            SorceCBox.Regions.Add("");
            // Load from file.
            if (File.Exists(PathFileAppRegions) == true)
            {
                string[] lines = File.ReadAllLines(PathFileAppRegions);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        SorceCBox.Regions.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppRegions));
                File.Create(PathFileAppRegions);
            }
        }
        public void Source_Load_Objectives()
        {
            // Clear list, because it may be reloaded later.
            SorceCBox.Objectives = new List<string>();
            // Add epmty string.
            SorceCBox.Objectives.Add("");
            // Load from file.
            if (File.Exists(PathFileAppObjectives) == true)
            {
                string[] lines = File.ReadAllLines(PathFileAppObjectives);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        SorceCBox.Objectives.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppObjectives));
                File.Create(PathFileAppObjectives);
            }
        }
        public void Source_Load_Warnings()
        {
            // Clear list, because it may be reloaded later.
            SorceCBox.Warnings = new List<string>();
            // Add epmty string.
            SorceCBox.Warnings.Add("");
            // Load from file.
            if (File.Exists(PathFileAppWarnings) == true)
            {
                string[] lines = File.ReadAllLines(PathFileAppWarnings);
                if (lines.Length > 0)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        SorceCBox.Warnings.Add(lines[i]);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppWarnings));
                File.Create(PathFileAppWarnings);
            }
        }
        #endregion
        #region Data
        // Versions.
        public void Data_Versions_Load()
        {
            // Clear the list, because function may be executed again.
            Data.Versions = new List<DataVersion>();
            // Create folder if it doesn't exist.
            Directory.CreateDirectory(PathFoldAppVersions);
            string[] pathFiles = Directory.GetFiles(PathFoldAppVersions, "*.ini", SearchOption.TopDirectoryOnly);
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
                        if (fileINI.ReadKeyString("Name", out string name, "Version") == true)
                        {
                            dataVersion.Name = name;
                        }
                        if (fileINI.ReadKeyString("Manifest", out string manifest, "Version") == true)
                        {
                            dataVersion.Manifest = manifest; // !!! Validate.
                        }
                        if (
                            fileINI.ReadKeyByte("ColorR", out Byte colorR, "Version") == true
                            &&
                            fileINI.ReadKeyByte("ColorG", out Byte colorG, "Version") == true
                            &&
                            fileINI.ReadKeyByte("ColorB", out Byte colorB, "Version") == true
                        )
                        {
                            dataVersion.Brush = new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB));
                        }
                        // Add data to the list.
                        Data.Versions.Add(dataVersion);
                    }
                }
            }
            // Load user data.
            DataUser_Versions_Load();
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
            if (inIdVersion >= 0 && Data.Versions.Count > 0)
            {
                outNumberVersion = Data.Versions[inIdVersion].Number;
            }
            return outNumberVersion;
        }
        public DataVersion Data_Versions_Get_Version(int inIdVersion)
        {
            DataVersion outDataVersion = new DataVersion();
            if (Data.Versions.Count > 0 && inIdVersion >= 0)
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
                Directory.CreateDirectory(PathFoldAppMods);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldAppMods, "*", SearchOption.TopDirectoryOnly);
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
                                            if (fileInfo.ReadKeyString("Name", out string name, "Mod") == true)
                                            {
                                                dataMod.Name = name;
                                            }
                                            if (fileInfo.ReadKeyInt("Time", out int time, "Mod") == true)
                                            {
                                                dataMod.Time = time;
                                            }
                                            if (fileInfo.ReadKeyString("Description", out string description, "Mod") == true)
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
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0 && inIdMod >= 0)
            {
                outNumberMod = Data.Versions[inIdVersion].Mods[inIdMod].Number;
            }
            return outNumberMod;
        }
        public DataMod Data_Mods_Get_Mod(int inIdVersion, int inIdMod)
        {
            DataMod outDataMod = new DataMod();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0 && inIdMod >= 0)
            {
                outDataMod = Data.Versions[inIdVersion].Mods[inIdMod];
            }
            return outDataMod;
        }
        public List<DataMod> Data_Mods_Get_Mods(int inIdVersion, string inText)
        {
            List<DataMod> outDataMods = new List<DataMod>();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0)
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
                Directory.CreateDirectory(PathFoldAppDives);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldAppDives, "*", SearchOption.TopDirectoryOnly);
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
                                        if (fileINI.ReadKeyUInt("Seed", out uint seed, "Dive") == true)
                                        {
                                            dataDive.Seed = seed;
                                        }
                                        dataDive.BrushBack = Data.Versions[idVersion].Brush;
                                        if (fileINI.ReadKeyString("Event", out string eventNumber, "Dive") == true)
                                        {
                                            dataDive.EventNumber = eventNumber;
                                        }
                                        if (fileINI.ReadKeyString("Date", out string date, "Dive") == true)
                                        {
                                            dataDive.Date = date;
                                        }
                                        if (fileINI.ReadKeyString("NormalName", out string normalName, "Dive") == true)
                                        {
                                            dataDive.Normal.Name = normalName;
                                        }
                                        if (fileINI.ReadKeyString("NormalRegion", out string normalRegion, "Dive") == true)
                                        {
                                            dataDive.Normal.Region = normalRegion;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionType1", out string normalMissionType1, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[0].Type = normalMissionType1;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionType2", out string normalMissionType2, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[1].Type = normalMissionType2;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionType3", out string normalMissionType3, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[2].Type = normalMissionType3;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionValue1", out string normalMissionValue1, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[0].Value = normalMissionValue1;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionValue2", out string normalMissionValue2, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[1].Value = normalMissionValue2;
                                        }
                                        if (fileINI.ReadKeyString("NormalMissionValue3", out string normalMissionValue3, "Dive") == true)
                                        {
                                            dataDive.Normal.Missions[2].Value = normalMissionValue3;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveType1", out string normalObjectiveType1, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[0].Type = normalObjectiveType1;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveType2", out string normalObjectiveType2, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[1].Type = normalObjectiveType2;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveType3", out string normalObjectiveType3, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[2].Type = normalObjectiveType3;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveValue1", out string normalObjectiveValue1, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[0].Value = normalObjectiveValue1;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveValue2", out string normalObjectiveValue2, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[1].Value = normalObjectiveValue2;
                                        }
                                        if (fileINI.ReadKeyString("NormalObjectiveValue3", out string normalObjectiveValue3, "Dive") == true)
                                        {
                                            dataDive.Normal.Objectives[2].Value = normalObjectiveValue3;
                                        }
                                        if (fileINI.ReadKeyString("NormalWarning1", out string normalWarning1, "Dive") == true)
                                        {
                                            dataDive.Normal.Warnings[0] = normalWarning1;
                                        }
                                        if (fileINI.ReadKeyString("NormalWarning2", out string normalWarning2, "Dive") == true)
                                        {
                                            dataDive.Normal.Warnings[1] = normalWarning2;
                                        }
                                        if (fileINI.ReadKeyString("NormalWarning3", out string normalWarning3, "Dive") == true)
                                        {
                                            dataDive.Normal.Warnings[2] = normalWarning3;
                                        }
                                        if (fileINI.ReadKeyString("NormalAnomaly1", out string normalAnomaly1, "Dive") == true)
                                        {
                                            dataDive.Normal.Anomalies[0] = normalAnomaly1;
                                        }
                                        if (fileINI.ReadKeyString("NormalAnomaly2", out string normalAnomaly2, "Dive") == true)
                                        {
                                            dataDive.Normal.Anomalies[1] = normalAnomaly2;
                                        }
                                        if (fileINI.ReadKeyString("NormalAnomaly3", out string normalAnomaly3, "Dive") == true)
                                        {
                                            dataDive.Normal.Anomalies[2] = normalAnomaly3;
                                        }
                                        if (fileINI.ReadKeyString("EliteName", out string eliteName, "Dive") == true)
                                        {
                                            dataDive.Elite.Name = eliteName;
                                        }
                                        if (fileINI.ReadKeyString("EliteRegion", out string eliteRegion, "Dive") == true)
                                        {
                                            dataDive.Elite.Region = eliteRegion;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionType1", out string eliteMissionType1, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[0].Type = eliteMissionType1;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionType2", out string eliteMissionType2, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[1].Type = eliteMissionType2;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionType3", out string eliteMissionType3, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[2].Type = eliteMissionType3;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionValue1", out string eliteMissionValue1, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[0].Value = eliteMissionValue1;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionValue2", out string eliteMissionValue2, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[1].Value = eliteMissionValue2;
                                        }
                                        if (fileINI.ReadKeyString("EliteMissionValue3", out string eliteMissionValue3, "Dive") == true)
                                        {
                                            dataDive.Elite.Missions[2].Value = eliteMissionValue3;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveType1", out string eliteObjectiveType1, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[0].Type = eliteObjectiveType1;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveType2", out string eliteObjectiveType2, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[1].Type = eliteObjectiveType2;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveType3", out string eliteObjectiveType3, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[2].Type = eliteObjectiveType3;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveValue1", out string eliteObjectiveValue1, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[0].Value = eliteObjectiveValue1;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveValue2", out string eliteObjectiveValue2, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[1].Value = eliteObjectiveValue2;
                                        }
                                        if (fileINI.ReadKeyString("EliteObjectiveValue3", out string eliteObjectiveValue3, "Dive") == true)
                                        {
                                            dataDive.Elite.Objectives[2].Value = eliteObjectiveValue3;
                                        }
                                        if (fileINI.ReadKeyString("EliteWarning1", out string eliteWarning1, "Dive") == true)
                                        {
                                            dataDive.Elite.Warnings[0] = eliteWarning1;
                                        }
                                        if (fileINI.ReadKeyString("EliteWarning2", out string eliteWarning2, "Dive") == true)
                                        {
                                            dataDive.Elite.Warnings[1] = eliteWarning2;
                                        }
                                        if (fileINI.ReadKeyString("EliteWarning3", out string eliteWarning3, "Dive") == true)
                                        {
                                            dataDive.Elite.Warnings[2] = eliteWarning3;
                                        }
                                        if (fileINI.ReadKeyString("EliteAnomaly1", out string eliteAnomaly1, "Dive") == true)
                                        {
                                            dataDive.Elite.Anomalies[0] = eliteAnomaly1;
                                        }
                                        if (fileINI.ReadKeyString("EliteAnomaly2", out string eliteAnomaly2, "Dive") == true)
                                        {
                                            dataDive.Elite.Anomalies[1] = eliteAnomaly2;
                                        }
                                        if (fileINI.ReadKeyString("EliteAnomaly3", out string eliteAnomaly3, "Dive") == true)
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
            if (inIdVersion >= 0 && inNumberDive != "")
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
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Dives.Count > 0 && inIdDive >= 0)
            {
                outNumberDive = Data.Versions[inIdVersion].Dives[inIdDive].Number;
            }
            return outNumberDive;
        }
        public DataDive Data_Dives_Get_Dive(int inIdVersion, int inIdDive)
        {
            DataDive outDataDive = new DataDive();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Dives.Count > 0 && inIdDive >= 0)
            {
                outDataDive = Data.Versions[inIdVersion].Dives[inIdDive];
            }
            return outDataDive;
        }
        public List<DataDive> Data_Dives_Get_Dives(int inIdVersion, string inText)
        {
            List<DataDive> outDataDives = new List<DataDive>();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Dives.Count > 0)
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
            List<DataDive> outDataDives = inDataDives.OrderByDescending(x => x.Number).ToList();
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
                Directory.CreateDirectory(PathFoldAppEvents);
                string[] pathFoldsVersion = Directory.GetDirectories(PathFoldAppEvents, "*", SearchOption.TopDirectoryOnly);
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
                                        FileINI fleINI = new FileINI(pathFilesEvent[iEvent]);
                                        // Define event.
                                        DataEvent dataEvent = new DataEvent();
                                        dataEvent.Number = numberEvent;
                                        if (fleINI.ReadKeyString("Name", out string name, "Event") == true)
                                        {
                                            dataEvent.Name = name;
                                        }
                                        if (fleINI.ReadKeyString("Date", out string date, "Event") == true)
                                        {
                                            dataEvent.Date = date; // Validate Date.
                                        }
                                        if (fleINI.ReadKeyString("Command", out string command, "Event") == true)
                                        {
                                            dataEvent.Command = command;
                                        }
                                        dataEvent.BrushBack = Data.Versions[idVersion].Brush;
                                        bool check = true;
                                        int count = 1;
                                        while (check == true)
                                        {
                                            if (fleINI.ReadKeyString("ItemName" + count, out string itemName, "Event") == true && fleINI.ReadKeyString("ItemType" + count, out string itemType, "Event") == true)
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
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Events.Count > 0 && inIdEvent >= 0)
            {
                outNumberEvent = Data.Versions[inIdVersion].Events[inIdEvent].Number;
            }
            return outNumberEvent;
        }
        public DataEvent Data_Events_Get_Event(int inIdVersion, int inIdEvent)
        {
            DataEvent outDataEvent = new DataEvent();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Events.Count > 0 && inIdEvent >= 0)
            {
                outDataEvent = Data.Versions[inIdVersion].Events[inIdEvent];
            }
            return outDataEvent;
        }
        public List<DataEvent> Data_Events_Get_Events(int inIdVersion, string inText)
        {
            List<DataEvent> outDataEvents = new List<DataEvent>();
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Events.Count > 0)
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
        #endregion
        #region DataUser
        // Versions.
        public void DataUser_Versions_Load()
        {
            // Create folder if it doesn't exist.
            Directory.CreateDirectory(PathFoldAppSaveVersions);
            // Get all files inside folder "Save\Versions".
            string[] pathFiles = Directory.GetFiles(PathFoldAppSaveVersions, "*.ini", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                for (int i = 0; i < pathFiles.Length; i++)
                {
                    // Get file name without extention.
                    string numberVersion = Path.GetFileNameWithoutExtension(pathFiles[i]);
                    // Check if data for version exists.
                    int idVersion = Data_Versions_Get_Id(numberVersion);
                    if (idVersion >= 0)
                    {
                        FileINI fileINI = new FileINI(pathFiles[i]);
                        if (fileINI.ReadKeyString("Path", out string path, "SaveVersion") == true)
                        {
                            Data.Versions[idVersion].Path = path; // No need to validate path (it will be validated on game launch).
                        }
                    }
                    else
                    {
                        File.Delete(pathFiles[i]);
                    }
                }
            }
        }
        #endregion
        #region Settings Load
        // Services.
        public void Settings_Load_Services_IP()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("IP", out string value, "Services") == true)
            {
                SettingsCurrent.Services.IP = value;
            }
        }
        public void Settings_Load_Services_ChangeRedirects()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("ChangeRedirects", out bool value, "Services") == true)
            {
                SettingsCurrent.Services.ChangeRedirects = value;
            }
        }
        public void Settings_Load_Services_ChangeCertificates()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("ChangeCertificates", out bool value, "Services") == true)
            {
                SettingsCurrent.Services.ChangeCertificates = value;
            }
        }
        public void Settings_Load_Services_StartServer()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("StartServer", out bool value, "Services") == true)
            {
                SettingsCurrent.Services.StartServer = value;
            }
        }
        // Version.
        public void Settings_Load_Version_Path()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("Path", out string value, "Version") == true)
            {
                SettingsCurrent.Version.Path = value;
            }
        }
        public void Settings_Load_Version_PlayerId()
        {
            if (File.Exists(PathFileGSEUser_Steam_Id) == true)
            {
                string[] lines = File.ReadAllLines(PathFileGSEUser_Steam_Id);
                // If file will be empty, it's length will be null and it will not load empty string.
                if (lines.Length == 1)
                {
                    SettingsCurrent.Version.PlayerId = lines[0];
                }
                else
                {
                    File_GSE_Write_Version_PlayerId(SettingsDefault.Version.PlayerId);
                }
            }
            else
            {
                File_GSE_Write_Version_PlayerId(SettingsDefault.Version.PlayerId);
            }
        }
        public void Settings_Load_Version_PlayerName()
        {
            if (File.Exists(PathFileGSEAccount_Name) == true)
            {
                string[] lines = File.ReadAllLines(PathFileGSEAccount_Name);
                // If file will be empty, it's length will be null and it will not load empty string.
                if (lines.Length == 1)
                {
                    SettingsCurrent.Version.PlayerName = lines[0];
                }
                else
                {
                    File_GSE_Write_Version_PlayerName(SettingsDefault.Version.PlayerName);
                }
            }
            else
            {
                File_GSE_Write_Version_PlayerName(SettingsDefault.Version.PlayerName);
            }
        }
        public void Settings_Load_Version_Command()
        {
            FileINI fileINI = new FileINI(PathFileGSEColdClientLoader);
            if (fileINI.ReadKeyString("ExeCommandLine", out string value, "SteamClient") == true)
            {
                SettingsCurrent.Version.Command = value;
            }
        }
        public void Settings_Load_Version_SelectedId()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("SelectedNumber", out string value, "Version") == true)
            {
                int selectedId = Data_Versions_Get_Id(value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Version.SelectedId = selectedId;
                    SettingsCurrent.Version.SelectedNumber = value;
                }
            }
        }
        // Mod.
        public void Settings_Load_Mod_Version_ModsIsEnabled()
        {
            for (int i = 0; i < Data.Versions.Count; i++)
            {
                Settings_Load_Mod_ModsIsEnabled(i);
            }
        }
        public void Settings_Load_Mod_ModsIsEnabled(int inId)
        {
            if (inId >= 0)
            {
                if (File.Exists(Data.Versions[inId].Path) == true)
                {
                    // "Path.GetFullPath" is used to convert "..\\" to go up the root.
                    // Notice that three "..\\" are used. One to remove "FSD-Win64-Shipping.exe", others to go up the root.
                    string pathFoldGameWindowsNoEditor = Path.GetFullPath(Path.Combine(Data.Versions[inId].Path, "..\\..\\..\\Saved\\Config\\WindowsNoEditor"));
                    string pathFile = Path.Combine(pathFoldGameWindowsNoEditor, "GameUserSettings.ini");
                    FileINI fileINI = new FileINI(pathFile);
                    for (int i = 0; i < Data.Versions[inId].Mods.Count; i++)
                    {
                        if (fileINI.ReadKeyBool(Data.Versions[inId].Mods[i].Number, out bool enabled, "/Script/FSD.UserGeneratedContent") == true)
                        {
                            Data.Versions[inId].Mods[i].IsEnabled = enabled;
                        }
                        else
                        {
                            Data.Versions[inId].Mods[i].IsEnabled = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Data.Versions[inId].Mods.Count; i++)
                    {
                        Data.Versions[inId].Mods[i].IsEnabled = false;
                    }
                }
            }
        }
        // Dive.
        public void Settings_Load_Dive_Seed()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyUInt("Seed", out uint value, "Dive") == true)
            {
                SettingsCurrent.Dive.Seed = value;
            }
        }
        public void Settings_Load_Dive_LostDives()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("LostDives", out bool value, "Dive") == true)
            {
                SettingsCurrent.Dive.LostDives = value;
            }
        }
        public void Settings_Load_Dive_SelectedId()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("SelectedNumber", out string value, "Dive") == true && SettingsCurrent.Version.SelectedId >= 0)
            {
                int selectedId = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Dive.SelectedId = selectedId;
                    SettingsCurrent.Dive.SelectedNumber = value;
                }
            }
        }
        // Event.
        public void Settings_Load_Event_Command()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("Command", out string value, "Event") == true)
            {
                SettingsCurrent.Event.Command = value;
            }
        }
        public void Settings_Load_Event_FreeBeers()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("FreeBeers", out bool value, "Event") == true)
            {
                SettingsCurrent.Event.FreeBeers = value;
            }
        }
        public void Settings_Load_Event_LostEvents()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyBool("LostEvents", out bool value, "Event") == true)
            {
                SettingsCurrent.Event.LostEvents = value;
            }
        }
        public void Settings_Load_Event_SelectedId()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyString("SelectedNumber", out string value, "Event") == true && SettingsCurrent.Version.SelectedId >= 0)
            {
                int selectedId = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, value);
                if (selectedId >= 0)
                {
                    SettingsCurrent.Event.SelectedId = selectedId;
                    SettingsCurrent.Event.SelectedNumber = value;
                }
            }
        }
        // Assignment.
        public void Settings_Load_Assignment_Seed()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyUInt("Seed", out uint value, "Assignment") == true)
            {
                SettingsCurrent.Assignment.Seed = value;
            }
        }
        // Common.
        public void Settings_Load_Common_PosX()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyDouble("PosX", out double value, "Common") == true)
            {
                SettingsCurrent.Common.PosX = value;
            }
        }
        public void Settings_Load_Common_PosY()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyDouble("PosY", out double value, "Common") == true)
            {
                SettingsCurrent.Common.PosY = value;
            }
        }
        public void Settings_Load_Common_SizeX()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyDouble("SizeX", out double value, "Common") == true)
            {
                SettingsCurrent.Common.SizeX = value;
            }
        }
        public void Settings_Load_Common_SizeY()
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            if (fileINI.ReadKeyDouble("SizeY", out double value, "Common") == true)
            {
                SettingsCurrent.Common.SizeY = value;
            }
        }
        #endregion
        #region Settings Save
        // Services.
        public void Settings_Save_Services_IP(string inIP)
        {
            // Internal Setting.
            SettingsCurrent.Services.IP = inIP;
            // External Setting.
            File_App_Write_Services_IP(inIP);
        }
        public void Settings_Save_Services_ChangeRedirects(bool inChangeRedirects)
        {
            // Internal Setting.
            SettingsCurrent.Services.ChangeRedirects = inChangeRedirects;
            // External Setting.
            File_App_Write_Services_ChangeRedirects(inChangeRedirects);
        }
        public void Settings_Save_Services_ChangeCertificates(bool inChangeCertificates)
        {
            // Internal Setting.
            SettingsCurrent.Services.ChangeCertificates = inChangeCertificates;
            // External Setting.
            File_App_Write_Services_ChangeCertificates(inChangeCertificates);
        }
        public void Settings_Save_Services_StartServer(bool inStartServer)
        {
            // Internal Setting.
            SettingsCurrent.Services.StartServer = inStartServer;
            // External Setting.
            File_App_Write_Services_StartServer(inStartServer);
        }
        // Version.
        public void Settings_Save_Version_Path(int inId, string inPath)
        {
            if (inId >= 0)
            {
                // Internal Setting.
                Data.Versions[inId].Path = inPath;
                // External Setting.
                if (inPath != "")
                {
                    File_DataUser_Write_Version_Path(inId, inPath);
                }
                else
                {
                    File_DataUser_Delete_Version_Path(inId);
                }
            }
            else
            {
                // Internal Setting.
                SettingsCurrent.Version.Path = inPath;
                // External Setting.
                File_App_Write_Version_Path(inPath);
            }
        }
        public void Settings_Save_Version_PlayerId(string inPlayerId)
        {
            // Internal Setting.
            SettingsCurrent.Version.PlayerId = inPlayerId;
            // External Setting.
            File_GSE_Write_Version_PlayerId(inPlayerId);
        }
        public void Settings_Save_Version_PlayerName(string inPlayerName)
        {
            // Internal Setting.
            SettingsCurrent.Version.PlayerName = inPlayerName;
            // External Setting.
            File_GSE_Write_Version_PlayerName(inPlayerName);
        }
        public void Settings_Save_Version_Command(string inCommand)
        {
            // Internal Setting.
            SettingsCurrent.Version.Command = inCommand;
            // External Setting.
            File_GSE_Write_Version_Command(inCommand);
        }
        public void Settings_Save_Version_Search(string inSearch)
        {
            // Internal Setting.
            SettingsCurrent.Version.Search = inSearch;
        }
        public void Settings_Save_Version_SelectedId(int inSelectedId)
        {
            // Internal Setting.
            SettingsCurrent.Version.SelectedId = inSelectedId;
            string inSelectedNumber = Data_Versions_Get_Number(inSelectedId);
            SettingsCurrent.Version.SelectedNumber = inSelectedNumber;
            // External Setting.
            File_App_Write_Version_SelectedNumber(inSelectedNumber);
        }
        // Mod.
        public void Settings_Save_Mod_Search(string inSearch)
        {
            // Internal Setting.
            SettingsCurrent.Mod.Search = inSearch;
        }
        public void Settings_Save_Mod_SelectedId(int inSelectedId)
        {
            // Internal Setting.
            SettingsCurrent.Mod.SelectedId = inSelectedId;
            string inSelectedNumber = Data_Mods_Get_Number(SettingsCurrent.Version.SelectedId, inSelectedId);
            SettingsCurrent.Mod.SelectedNumber = inSelectedNumber;
        }
        public void Settings_Save_Mod_ModIsEnabled(int inIdVersion, int inIdMod, bool inModIsEnabled)
        {
            Data.Versions[inIdVersion].Mods[inIdMod].IsEnabled = inModIsEnabled;
        }
        // Dive.
        public void Settings_Save_Dive_Seed(uint inSeed)
        {
            // Internal Setting.
            SettingsCurrent.Dive.Seed = inSeed;
            // External Setting.
            File_App_Write_Dive_Seed(inSeed);
            File_Apache24_Write_Dive_Seed(inSeed);
        }
        public void Settings_Save_Dive_LostDives(bool inLostDives)
        {
            // Internal Setting.
            SettingsCurrent.Dive.LostDives = inLostDives;
            // External Setting.
            File_App_Write_Dive_LostDives(inLostDives);
        }
        public void Settings_Save_Dive_Search(string inSearch)
        {
            // Internal Setting.
            SettingsCurrent.Dive.Search = inSearch;
        }
        public void Settings_Save_Dive_SelectedId(int inSelectedId)
        {
            // Internal Setting.
            SettingsCurrent.Dive.SelectedId = inSelectedId;
            string inSelectedNumber = Data_Dives_Get_Number(SettingsCurrent.Version.SelectedId, inSelectedId);
            SettingsCurrent.Dive.SelectedNumber = inSelectedNumber;
            // External Setting.
            File_App_Write_Dive_SelectedNumber(inSelectedNumber);
        }
        // Event.
        public void Settings_Save_Event_Command(string inCommand)
        {
            // Internal Setting.
            SettingsCurrent.Event.Command = inCommand;
            // External Setting.
            File_App_Write_Event_Command(inCommand);
            File_Apache24_Write_Event_Command(inCommand);
        }
        public void Settings_Save_Event_FreeBeers(bool inFreeBeers)
        {
            // Internal Setting.
            SettingsCurrent.Event.FreeBeers = inFreeBeers;
            // External Setting.
            File_App_Write_Event_FreeBeers(inFreeBeers);
            File_Apache24_Write_Event_FreeBeers(inFreeBeers);
        }
        public void Settings_Save_Event_LostEvents(bool inLostEvents)
        {
            // Internal Setting.
            SettingsCurrent.Event.LostEvents = inLostEvents;
            // External Setting.
            File_App_Write_Event_LostEvents(inLostEvents);
        }
        public void Settings_Save_Event_Search(string inSearch)
        {
            // Internal Setting.
            SettingsCurrent.Event.Search = inSearch;
        }
        public void Settings_Save_Event_SelectedId(int inSelectedId)
        {
            // Internal Setting.
            SettingsCurrent.Event.SelectedId = inSelectedId;
            string inSelectedNumber = Data_Events_Get_Number(SettingsCurrent.Version.SelectedId, inSelectedId);
            SettingsCurrent.Event.SelectedNumber = inSelectedNumber;
            // External Setting.
            File_App_Write_Event_SelectedNumber(inSelectedNumber);
        }
        // Assignment.
        public void Settings_Save_Assignment_Seed(uint inSeed)
        {
            // Internal Setting.
            SettingsCurrent.Assignment.Seed = inSeed;
            // External Setting.
            File_App_Write_Assignment_Seed(inSeed);
            File_Apache24_Write_Assignment_Seed(inSeed);
        }
        // Common.
        public void Settings_Save_Common_PosX(double inPosX)
        {
            // Internal Setting.
            SettingsCurrent.Common.PosX = inPosX;
            // External Setting.
            File_App_Write_Common_PosX(inPosX);
        }
        public void Settings_Save_Common_PosY(double inPosY)
        {
            // Internal Setting.
            SettingsCurrent.Common.PosY = inPosY;
            // External Setting.
            File_App_Write_Common_PosY(inPosY);
        }
        public void Settings_Save_Common_SizeX(double inSizeX)
        {
            // Internal Setting.
            SettingsCurrent.Common.SizeX = inSizeX;
            // External Setting.
            File_App_Write_Common_SizeX(inSizeX);
        }
        public void Settings_Save_Common_SizeY(double inSizeY)
        {
            // Internal Setting.
            SettingsCurrent.Common.SizeY = inSizeY;
            // External Setting.
            File_App_Write_Common_SizeY(inSizeY);
        }
        #endregion
        #region File App
        // App.
        public void File_App_Write_Services_IP(string inIP)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("IP", inIP, "Services");
        }
        public void File_App_Write_Services_ChangeRedirects(bool inChangeRedirects)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("ChangeRedirects", inChangeRedirects.ToString(), "Services");
        }
        public void File_App_Write_Services_ChangeCertificates(bool inChangeCertificates)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("ChangeCertificates", inChangeCertificates.ToString(), "Services");
        }
        public void File_App_Write_Services_StartServer(bool inStartServer)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("StartServer", inStartServer.ToString(), "Services");
        }
        public void File_App_Write_Version_Path(string inPath)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("Path", inPath, "Version");
        }
        public void File_App_Write_Version_SelectedNumber(string inSelectedNumber)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("SelectedNumber", inSelectedNumber, "Version");
        }
        public void File_App_Write_Dive_Seed(uint inSeed)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("Seed", inSeed.ToString(), "Dive");
        }
        public void File_App_Write_Dive_LostDives(bool inLostDives)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("LostDives", inLostDives.ToString(), "Dive");
        }
        public void File_App_Write_Dive_SelectedNumber(string inSelectedNumber)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("SelectedNumber", inSelectedNumber, "Dive");
        }
        public void File_App_Write_Event_Command(string inCommand)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("Command", inCommand, "Event");
        }
        public void File_App_Write_Event_FreeBeers(bool inFreeBeers)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("FreeBeers", inFreeBeers.ToString(), "Event");
        }
        public void File_App_Write_Event_LostEvents(bool inLostEvents)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("LostEvents", inLostEvents.ToString(), "Event");
        }
        public void File_App_Write_Event_SelectedNumber(string inSelectedNumber)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("SelectedNumber", inSelectedNumber, "Event");
        }
        public void File_App_Write_Assignment_Seed(uint inSeed)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("Seed", inSeed.ToString(), "Assignment");
        }
        public void File_App_Write_Common_PosX(double inPosX)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("PosX", inPosX.ToString(), "Common");
        }
        public void File_App_Write_Common_PosY(double inPosY)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("PosY", inPosY.ToString(), "Common");
        }
        public void File_App_Write_Common_SizeX(double inSizeX)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("SizeX", inSizeX.ToString(), "Common");
        }
        public void File_App_Write_Common_SizeY(double inSizeY)
        {
            FileINI fileINI = new FileINI(PathFileAppSettings);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileAppSettings));
            fileINI.WriteKey("SizeY", inSizeY.ToString(), "Common");
        }
        #endregion
        #region File DataUser
        public void File_DataUser_Write_Version_Path(int inId, string inPath)
        {
            Directory.CreateDirectory(PathFoldAppSaveVersions);
            FileINI fileINI = new FileINI(Path.Combine(PathFoldAppSaveVersions, Data.Versions[inId].Number + ".ini"));
            fileINI.WriteKey("Path", inPath, "SaveVersion");
        }
        public void File_DataUser_Delete_Version_Path(int inId)
        {
            File.Delete(Path.Combine(PathFoldAppSaveVersions, Data.Versions[inId].Number + ".ini"));
        }
        #endregion
        #region File Apache24
        public void File_Apache24_Write_Dive_Seed(uint inSeed)
        {
            string command = "{\"Seed\":" + inSeed + ",\"SeedV2\":" + inSeed + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
            // Create folder for file.
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24DeepDive1));
            File.WriteAllText(PathFileApache24DeepDive1, command);
            // Create folder for file.
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24DeepDive2));
            File.WriteAllText(PathFileApache24DeepDive2, command);
        }
        public void File_Apache24_Write_Event_Command(string inCommand)
        {
            // Remove spaces and replace "comma" with "quotes,comma,qoutes".
            string command = "{\"ActiveEvents\":[\"" + Regex.Replace(inCommand, @" ", "").Replace(",", "\",\"") + "\"]}";
            // Create folder for file.
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24Events1));
            File.WriteAllText(PathFileApache24Events1, command);
            // Create folder for file.
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24Events2));
            File.WriteAllText(PathFileApache24Events2, command);
        }
        public void File_Apache24_Write_Event_FreeBeers(bool inFreeBeers)
        {
            string command = "{\"FreeBeers\":" + inFreeBeers + "}";
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24CGoalStateTime1));
            File.WriteAllText(PathFileApache24CGoalStateTime1, command);
        }
        public void File_Apache24_Write_Assignment_Seed(uint inSeed)
        {
            string command = "{\"Seed\":" + inSeed + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24Weekly1));
            File.WriteAllText(PathFileApache24Weekly1, command);
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileApache24Weekly2));
            File.WriteAllText(PathFileApache24Weekly2, command);
        }
        #endregion
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
        private static string String_FormatTo_Literal(string inText)
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
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Fix file not having empty line at the end and add lines between redirects.
                if (File.ReadAllText(PathFileWindowsHosts).EndsWith("\r\n") == false)
                {
                    // Add two lines, first for not writing redirect on top of existing one, second for space between redirects.
                    File.AppendAllText(PathFileWindowsHosts, Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    // Add one line for space between redirects.
                    File.AppendAllText(PathFileWindowsHosts, Environment.NewLine);
                }
                // Add label for app redirects.
                File.AppendAllText(PathFileWindowsHosts, "#DeepDiveEmulator Redirects" + Environment.NewLine);
                // Add redirects.
                foreach (string pathFile in pathFiles)
                {
                    // Define redirect name.
                    string hostName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                    // Define command.
                    string command = SettingsCurrent.Services.IP + " " + hostName;
                    // Write command down at the end of all lines.
                    File.AppendAllText(PathFileWindowsHosts, command + Environment.NewLine);
                }
            }
        }
        public void Server_Redirects_Remove()
        {
            Directory.CreateDirectory(PathFoldApache24Certificates);
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                string[] lines = File.ReadAllLines(PathFileWindowsHosts);
                List<string> tempLines = new List<string>();
                // Remove "#DeepDiveEmulator Redirects".
                foreach (string line in lines)
                {
                    if (line.Contains("#DeepDiveEmulator Redirects") == false)
                    {
                        tempLines.Add(line);
                    }
                    else
                    {
                        // Remove last line if it is empty, because it was added with redirects for spacing them out.
                        if (tempLines[tempLines.Count - 1] == "")
                        {
                            tempLines.RemoveAt(tempLines.Count - 1);
                        }
                    }
                }
                // Update lines.
                lines = tempLines.ToArray();
                // Clear tempLines.
                tempLines.Clear();
                // Remove redirects.
                foreach (string pathFile in pathFiles)
                {
                    string hostName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                    //
                    foreach (string line in lines)
                    {
                        if (line.Contains(hostName) == false)
                        {
                            tempLines.Add(line);
                        }
                    }
                    // Update lines.
                    lines = tempLines.ToArray();
                    // Clear tempLines.
                    tempLines.Clear();
                }
                // Write down all lines, without wrong redirects.
                File.WriteAllLines(PathFileWindowsHosts, lines);
            }
        }
        public bool Server_Redirects_Check()
        {
            string redirectsIP = "";
            //
            Directory.CreateDirectory(PathFoldApache24Certificates);
            string[] pathFiles = Directory.GetFiles(PathFoldApache24Certificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Count for the ammount of current redirects with required host name.
                int count = 0;
                //
                foreach (string pathFile in pathFiles)
                {
                    string hostName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.

                    //
                    string[] lines = File.ReadAllLines(PathFileWindowsHosts);
                    foreach (string line in lines)
                    {
                        if (line.Contains(hostName) == true)
                        {
                            // Get IP only on first pass and check with others.
                            if (count == 0)
                            {
                                redirectsIP = Regex.Replace(line, @" ", "").Replace(hostName, ""); // Remove spaces and remove host name.
                                if (IPAddress.TryParse(redirectsIP, out IPAddress ipAddress) == false)
                                {
                                    redirectsIP = "";
                                    break;
                                }
                            }
                            else
                            {
                                if (Regex.Replace(line, @" ", "").Replace(hostName, "") != redirectsIP)
                                {
                                    redirectsIP = "";
                                    break;
                                }
                            }
                            // Increase ammount of current redirects with correct host name.
                            count = count + 1;
                        }
                    }
                }
                // Check if ammount of current redirects is equal to amount of files.
                if (count == pathFiles.Length && redirectsIP != "")
                {
                    // All required redirects are added.
                    return true;
                }
                else
                {
                    // One or more required redirects are not added.
                    return false;
                }
            }
            else
            {
                // Folder does not contain certificates to check with.
                return false;
            }
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
            Process process = new Process();
            process.StartInfo.FileName = PathFileHTTPDEXE;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
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
        #region Game
        public void Game_Save(int inIdVersion)
        {
            File_Game_Delete_Section(inIdVersion);
            //
            File_Game_Write_ModsAreEnabled(inIdVersion);
            File_Game_Write_CheckGameversion(inIdVersion, "False");
            File_Game_Write_CurrentBranchName(inIdVersion, "public");
        }
        public void File_Game_Write_ModsAreEnabled(int inIdVersion)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                //
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                FileINI fileINI = new FileINI(pathFile);
                if (Data.Versions[inIdVersion].Mods.Count > 0)
                {
                    for (int i = 0; i < Data.Versions[inIdVersion].Mods.Count; i++)
                    {
                        if (Data.Versions[inIdVersion].Mods[i].IsEnabled == true)
                        {
                            fileINI.WriteKey(Data.Versions[inIdVersion].Mods[i].Number, Data.Versions[inIdVersion].Mods[i].IsEnabled.ToString(), "/Script/FSD.UserGeneratedContent");
                        }
                    }
                }
            }
        }
        public void File_Game_Write_CheckGameversion(int inIdVersion, string inCheckGameversion)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                //
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                FileINI fileINI = new FileINI(pathFile);
                fileINI.WriteKey("CheckGameversion", inCheckGameversion, "/Script/FSD.UserGeneratedContent");
            }
        }
        public void File_Game_Write_CurrentBranchName(int inIdVersion, string inCurrentBranchName)
        {
            if (inIdVersion >= 0)
            {
                string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Data.Versions[inIdVersion].Path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
                //
                Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
                FileINI fileINI = new FileINI(pathFile);
                fileINI.WriteKey("CurrentBranchName", inCurrentBranchName, "/Script/FSD.UserGeneratedContent");
            }
        }
        public void File_Game_Delete_Section(int inIdVersion)
        {
            string path;
            if (inIdVersion >= 0)
            {
                path = Data.Versions[inIdVersion].Path;
            }
            else
            {
                path = SettingsCurrent.Version.Path;
            }
            string pathFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..\\..\\Saved\\Config\\WindowsNoEditor\\GameUserSettings.ini"));
            //
            Directory.CreateDirectory(Path.GetDirectoryName(pathFile));
            FileINI fileINI = new FileINI(pathFile);
            fileINI.DeleteSection("/Script/FSD.UserGeneratedContent");
        }
        #endregion
        #region ModIO
        public void ModIO_Save(int inIdVersion)
        {
            File_ModIO_Write_State(inIdVersion);
            File_ModIO_Write_GlobalSettings();
            File_ModIO_Write_User(inIdVersion);
        }
        public void File_ModIO_Write_State(int inIdVersion)
        {
            ModIOState data = new ModIOState();
            //
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0)
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
                        mod.PathOnDisk = Path.Combine(PathFoldAppMods, numberVersion, Data.Versions[inIdVersion].Mods[i].Number);
                        mod.Profile.tags[1].name = numberVersionParts[0] + "." + numberVersionParts[1];
                        //
                        data.Mods.Add(mod);
                    }
                }
            }
            //
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileModIOState));
            File.WriteAllText(PathFileModIOState, JsonSerializer.Serialize(data));
        }
        public void File_ModIO_Write_GlobalSettings()
        {
            ModIOGlobalSettings data = new ModIOGlobalSettings();
            //
            data.RootLocalStoragePath = PathFoldModIO;
            //
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileModIOGlobalSettings));
            File.WriteAllText(PathFileModIOGlobalSettings, JsonSerializer.Serialize(data));
        }
        public void File_ModIO_Write_User(int inIdVersion)
        {
            ModIOUser data = new ModIOUser();
            //
            if (inIdVersion >= 0 && Data.Versions[inIdVersion].Mods.Count > 0)
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
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileModIOUser));
            File.WriteAllText(PathFileModIOUser, JsonSerializer.Serialize(data));
        }
        #endregion
        #region GoldbergSteamEmulator
        public bool? GSE_Path_Check(string inPath)
        {
            bool? check;
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
            else
            {
                check = null;
            }
            return check;
        }
        public void GSE_Launch(int inIdVersion)
        {
            File_GSE_Write_Essentials(inIdVersion);
            if (File.Exists(PathFileGSESteamClient_Loader) == true)
            {
                Process.Start(PathFileGSESteamClient_Loader);
            }
        }
        public void File_GSE_Write_Essentials(int inIdVersion)
        {
            string path;
            if (inIdVersion >= 0)
            {
                path = Data.Versions[inIdVersion].Path;
            }
            else
            {
                path = SettingsCurrent.Version.Path;
            }
            string pathFileSCDLL = Path.Combine(PathFoldGSE, @"steamclient.dll");
            string pathFileSC64DLL = Path.Combine(PathFoldGSE, @"steamclient64.dll");
            //
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileGSEColdClientLoader));
            FileINI fileINI = new FileINI(PathFileGSEColdClientLoader);
            fileINI.WriteKey("Exe", path, "SteamClient");
            fileINI.WriteKey("SteamClientDll", pathFileSCDLL, "SteamClient");
            fileINI.WriteKey("SteamClient64Dll", pathFileSC64DLL, "SteamClient");
        }
        public void File_GSE_Write_Version_PlayerId(string inPlayerId)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileGSEUser_Steam_Id));
            File.WriteAllText(PathFileGSEUser_Steam_Id, inPlayerId);
        }
        public void File_GSE_Write_Version_PlayerName(string inPlayerName)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileGSEAccount_Name));
            File.WriteAllText(PathFileGSEAccount_Name, inPlayerName);
        }
        public void File_GSE_Write_Version_Command(string inCommand)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PathFileGSEColdClientLoader));
            FileINI fileINI = new FileINI(PathFileGSEColdClientLoader);
            fileINI.WriteKey("ExeCommandLine", inCommand, "SteamClient");
        }
        #endregion
        #region Steam
        public void Steam_OpenConsole()
        {
            Process.Start("explorer.exe", "steam://nav/console");
        }
        public bool Steam_CheckRunning()
        {
            Process[] processes = Process.GetProcessesByName("Steam");
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
            VListVers.ItemsSource = SorceVlistVersions;
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
            SorceVlistVersions.Clear();
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
                SorceVlistVersions.Add(srcVListVersion);
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
            string numberMod = Data_Mods_Get_Number(idVersionOld, SettingsCurrent.Mod.SelectedId);
            int idMod = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, numberMod);
            VListMods_SelectId(idMod, true);
            VListMods_Fill();
            //
            string numberDive = Data_Dives_Get_Number(idVersionOld, SettingsCurrent.Dive.SelectedId);
            int idDive = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, numberDive);
            VListDivs_SelectId(idDive, true);
            VListDivs_Fill();
            //
            string numberEvent = Data_Events_Get_Number(idVersionOld, SettingsCurrent.Event.SelectedId);
            int idEvent = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, numberEvent);
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
            VListMods.ItemsSource = SorceVlistMods;
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
            SorceVlistMods.Clear();
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
                    SorceVlistMods.Add(srcVListMod);
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
            CBoxDDNorReg.ItemsSource = SorceCBox.Regions;
            CBoxDDNorReg.SelectedIndex = 0;
            CBoxDDNorMisT1.ItemsSource = SorceCBox.Missions;
            CBoxDDNorMisT1.SelectedIndex = 0;
            CBoxDDNorMisT2.ItemsSource = SorceCBox.Missions;
            CBoxDDNorMisT2.SelectedIndex = 0;
            CBoxDDNorMisT3.ItemsSource = SorceCBox.Missions;
            CBoxDDNorMisT3.SelectedIndex = 0;
            CBoxDDNorObjT1.ItemsSource = SorceCBox.Objectives;
            CBoxDDNorObjT1.SelectedIndex = 0;
            CBoxDDNorObjT2.ItemsSource = SorceCBox.Objectives;
            CBoxDDNorObjT2.SelectedIndex = 0;
            CBoxDDNorObjT3.ItemsSource = SorceCBox.Objectives;
            CBoxDDNorObjT3.SelectedIndex = 0;
            CBoxDDNorWar1.ItemsSource = SorceCBox.Warnings;
            CBoxDDNorWar1.SelectedIndex = 0;
            CBoxDDNorWar2.ItemsSource = SorceCBox.Warnings;
            CBoxDDNorWar2.SelectedIndex = 0;
            CBoxDDNorWar3.ItemsSource = SorceCBox.Warnings;
            CBoxDDNorWar3.SelectedIndex = 0;
            CBoxDDNorAno1.ItemsSource = SorceCBox.Anomalies;
            CBoxDDNorAno1.SelectedIndex = 0;
            CBoxDDNorAno2.ItemsSource = SorceCBox.Anomalies;
            CBoxDDNorAno2.SelectedIndex = 0;
            CBoxDDNorAno3.ItemsSource = SorceCBox.Anomalies;
            CBoxDDNorAno3.SelectedIndex = 0;
            CBoxDDEliReg.ItemsSource = SorceCBox.Regions;
            CBoxDDEliReg.SelectedIndex = 0;
            CBoxDDEliMisT1.ItemsSource = SorceCBox.Missions;
            CBoxDDEliMisT1.SelectedIndex = 0;
            CBoxDDEliMisT2.ItemsSource = SorceCBox.Missions;
            CBoxDDEliMisT2.SelectedIndex = 0;
            CBoxDDEliMisT3.ItemsSource = SorceCBox.Missions;
            CBoxDDEliMisT3.SelectedIndex = 0;
            CBoxDDEliObjT1.ItemsSource = SorceCBox.Objectives;
            CBoxDDEliObjT1.SelectedIndex = 0;
            CBoxDDEliObjT2.ItemsSource = SorceCBox.Objectives;
            CBoxDDEliObjT2.SelectedIndex = 0;
            CBoxDDEliObjT3.ItemsSource = SorceCBox.Objectives;
            CBoxDDEliObjT3.SelectedIndex = 0;
            CBoxDDEliWar1.ItemsSource = SorceCBox.Warnings;
            CBoxDDEliWar1.SelectedIndex = 0;
            CBoxDDEliWar2.ItemsSource = SorceCBox.Warnings;
            CBoxDDEliWar2.SelectedIndex = 0;
            CBoxDDEliWar3.ItemsSource = SorceCBox.Warnings;
            CBoxDDEliWar3.SelectedIndex = 0;
            CBoxDDEliAno1.ItemsSource = SorceCBox.Anomalies;
            CBoxDDEliAno1.SelectedIndex = 0;
            CBoxDDEliAno2.ItemsSource = SorceCBox.Anomalies;
            CBoxDDEliAno2.SelectedIndex = 0;
            CBoxDDEliAno3.ItemsSource = SorceCBox.Anomalies;
            CBoxDDEliAno3.SelectedIndex = 0;
            //
            TmrDivsSearch.Tick += TmrDivsSearch_Tick;
            TmrDivsSearch.Interval = TimeSpan.FromSeconds(0.5);
            //
            VListDivs.ItemsSource = SorceVlistDives;
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
            SorceVlistDives.Clear();
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
                SorceVlistDives.Add(srcVListDive);
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
            VListEvts.ItemsSource = SorceVlistEvents;
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
            SorceVlistEvents.Clear();
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
                SorceVlistEvents.Add(srcVListEvent);
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
            string pathFile = Path.Combine(PathFoldApp, @"Languages\English\Help.txt");
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
            Source_Load_Anomalies();
            Source_Load_Missions();
            Source_Load_Regions();
            Source_Load_Objectives();
            Source_Load_Warnings();
            //
            Data_Versions_Load();
            Data_Mods_Load();
            Data_Dives_Load();
            Data_Events_Load();
            // Settings.
            Settings_Load_Services_IP();
            Settings_Load_Services_ChangeRedirects();
            Settings_Load_Services_ChangeCertificates();
            Settings_Load_Services_StartServer();
            //
            Settings_Load_Version_Path();
            Settings_Load_Version_PlayerId();
            Settings_Load_Version_PlayerName();
            Settings_Load_Version_Command();
            Settings_Load_Version_SelectedId();
            //
            Settings_Load_Mod_Version_ModsIsEnabled();
            // Dive.
            Settings_Load_Dive_LostDives();
            Settings_Load_Dive_Seed();
            Settings_Load_Dive_SelectedId();
            // Event.
            Settings_Load_Event_Command();
            Settings_Load_Event_FreeBeers();
            Settings_Load_Event_LostEvents();
            Settings_Load_Event_SelectedId();
            // Assignment.
            Settings_Load_Assignment_Seed();
            // Common.
            Settings_Load_Common_PosX();
            Settings_Load_Common_PosY();
            Settings_Load_Common_SizeX();
            Settings_Load_Common_SizeY();


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
            bool changeRedirects = CBoxSvcsRedsChange.IsChecked.GetValueOrDefault();
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
            bool changeCerts = CBoxSvcsCertsChange.IsChecked.GetValueOrDefault();
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
            bool startOnLaunch = CBoxSvcsServChange.IsChecked.GetValueOrDefault();
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
            Settings_Load_Mod_ModsIsEnabled(idVersion);
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
                bool? check = GSE_Path_Check(path);
                if (check != null)
                {
                    if (check == true)
                    {
                        if (SettingsCurrent.Version.PlayerId != null)
                        {
                            ModIO_Save(SettingsCurrent.Version.SelectedId);
                            Game_Save(SettingsCurrent.Version.SelectedId);
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
                int idVersion = Data_Versions_Get_Id(SorceVlistVersions[VListVers.SelectedIndex].Number);
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
                                                        string pathFoldModNew = Path.Combine(PathFoldAppSaveMods, SettingsCurrent.Version.SelectedNumber, numberMod);
                                                        Directory.CreateDirectory(pathFoldModNew);
                                                        File.Copy(pathFilesMod[0], Path.Combine(pathFoldModNew, "File.pak"));
                                                        string PathFileInfo = Path.Combine(pathFoldModNew, "Info.ini");
                                                        FileINI fileINI = new FileINI(PathFileInfo);
                                                        fileINI.WriteKey("Name", nameMod, "Mod");
                                                        fileINI.WriteKey("Time", timeMod.ToString(), "Mod");
                                                        fileINI.WriteKey("Description", descriptionMod, "Mod");
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
                int idMod = Data_Mods_Get_Id(SettingsCurrent.Version.SelectedId, SorceVlistMods[VListMods.SelectedIndex].Number);
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
                    File_Apache24_Write_Dive_Seed(seed);
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
            bool setting = CBoxDivsLost.IsChecked.GetValueOrDefault();
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
                int idDive = Data_Dives_Get_Id(SettingsCurrent.Version.SelectedId, SorceVlistDives[VListDivs.SelectedIndex].Number);
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
                File_Apache24_Write_Event_Command(command);
            }
            else
            {
                Settings_Save_Event_Command(command);
            }
        }
        private void CBoxEvtsFreeBeers_Click(object sender, RoutedEventArgs e)
        {
            bool freeBeers = CBoxEvtsFreeBeers.IsChecked.GetValueOrDefault();
            Settings_Save_Event_FreeBeers(freeBeers);
        }
        private void CBoxEvtsLost_Click(object sender, RoutedEventArgs e)
        {
            bool setting = CBoxEvtsLost.IsChecked.GetValueOrDefault();
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
                int idEvent = Data_Events_Get_Id(SettingsCurrent.Version.SelectedId, SorceVlistEvents[VListEvts.SelectedIndex].Number);
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
    }
}
