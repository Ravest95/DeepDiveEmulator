using DeepDiveEmulator.ClassData;
using DeepDiveEmulator.ClassDataEnum;
using DeepDiveEmulator.ClassDataSettings;
using DeepDiveEmulator.ClassFileINI;

using Application = System.Windows.Forms.Application;
using Path = System.IO.Path;
using Window = System.Windows.Window;
using Clipboard = System.Windows.Clipboard;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Text;
using System.Collections.ObjectModel;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Documents;
using DataFormats = System.Windows.DataFormats;
using static System.Windows.Forms.DataFormats;
using System.Collections;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace DeepDiveEmulator
{
    public partial class MainWindow : Window
    {
        #region ResizeWindows
        bool ResizeInProcess = false;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Rectangle senderRect = sender as System.Windows.Shapes.Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = true;
            }
        }
        private void Resizeing_Form(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ResizeInProcess)
            {
                System.Windows.Shapes.Rectangle senderRect = sender as System.Windows.Shapes.Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    senderRect.CaptureMouse();
                    double vectorX = e.GetPosition(mainWindow).X;
                    double vectorY = e.GetPosition(mainWindow).Y;
                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        if (vectorX < 0)
                        {
                            mainWindow.Width = mainWindow.Width + Math.Abs(vectorX);
                            mainWindow.Left = mainWindow.Left - Math.Abs(vectorX);
                        }
                        else
                        {
                            // Define new width.
                            double windowSizeX = mainWindow.Width - vectorX;
                            // Check if width is bigger then minimal width.
                            if (windowSizeX > mainWindow.MinWidth)
                            {
                                mainWindow.Left = mainWindow.Left + vectorX;
                                mainWindow.Width = windowSizeX;
                            }
                            else
                            {
                                mainWindow.Left = mainWindow.Left + mainWindow.Width - mainWindow.MinWidth;
                                mainWindow.Width = mainWindow.MinWidth;
                            }
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        // Define true vector, without window width.
                        vectorX = vectorX - mainWindow.Width;
                        if (vectorX > 0)
                        {
                            mainWindow.Width = mainWindow.Width + vectorX;
                        }
                        else
                        {
                            // Define new width.
                            double windowSizeX = mainWindow.Width - Math.Abs(vectorX);
                            // Check if width is bigger then minimal width.
                            if (windowSizeX > mainWindow.MinWidth)
                            {
                                mainWindow.Width = windowSizeX;
                            }
                            else
                            {
                                mainWindow.Width = mainWindow.MinWidth;
                            }
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        if (vectorY < 0)
                        {
                            mainWindow.Height = mainWindow.Height + Math.Abs(vectorY);
                            mainWindow.Top = mainWindow.Top - Math.Abs(vectorY);
                        }
                        else if (vectorY > 0)
                        {
                            // Define new heigh.
                            double windowSizeY = mainWindow.Height - vectorY;
                            // Check if height is bigger then minimal height.
                            if (windowSizeY > mainWindow.MinHeight)
                            {
                                mainWindow.Top = mainWindow.Top + vectorY;
                                mainWindow.Height = windowSizeY;
                            }
                            else if (windowSizeY < mainWindow.MinHeight)
                            {
                                mainWindow.Top = mainWindow.Top + mainWindow.Height - mainWindow.MinHeight;
                                mainWindow.Height = mainWindow.MinHeight;
                            }
                        }
                    }
                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        // Define true vector, without window height.
                        vectorY = vectorY - mainWindow.Height;
                        if (vectorY > 0)
                        {
                            mainWindow.Height = mainWindow.Height + vectorY;
                        }
                        else
                        {
                            double windowSizeY = mainWindow.Height - Math.Abs(vectorY);
                            if (windowSizeY > mainWindow.MinHeight)
                            {
                                mainWindow.Height = windowSizeY;
                            }
                            else
                            {
                                mainWindow.Height = mainWindow.MinHeight;
                            }
                        }
                    }
                }
            }
        }
        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Rectangle senderRect = sender as System.Windows.Shapes.Rectangle;
            if (senderRect != null)
            {
                ResizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }
        #endregion
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
                MaximizeButton.Content = "";
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
                MaximizeButton.Content = "";
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


        // Application main path.
        public static string PathFolderApplication = Application.StartupPath;
        // Application path folders.
        public static string PathFolderDataEvents = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ViewLists\EventsStandard");
        public static string PathFolderDataGameVersions = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ViewLists\GameVersionsStandard");
        public static string PathFolderDataDeepDivesStandard = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ViewLists\DeepDivesStandard");
        public static string PathFolderApache24 = Path.Combine(PathFolderApplication, @"Apache24");
        public static string PathFolderGoldbergSteamEmulator = Path.Combine(PathFolderApplication, @"GoldbergSteamEmulator");
        // Application path files.
        public static string PathFileEnumAnomalies = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ComboBoxes\Anomalies");
        public static string PathFileEnumMissions = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ComboBoxes\Missions");
        public static string PathFileEnumRegions = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ComboBoxes\Regions");
        public static string PathFileEnumSideObjectives = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ComboBoxes\SideObjectives");
        public static string PathFileEnumWarnings = Path.Combine(PathFolderApplication, @"Data\ItemsSources\ComboBoxes\Warnings");
        // Apache24 path folders.
        public static string PathFolderWebsiteCertificates = Path.Combine(PathFolderApache24, @"conf\ssl.crt");
        // Apache24 path files.
        public static string PathFileHTTPD = Path.Combine(PathFolderApache24, @"bin\httpd.exe");
        public static string PathFileHTTPDConfig = Path.Combine(PathFolderApache24, @"conf\httpd.conf");
        public static string PathFileDeepDive1 = Path.Combine(PathFolderApache24, @"htdocs\drg.ghostship.dk\events\deepdive");
        public static string PathFileDeepDive2 = Path.Combine(PathFolderApache24, @"htdocs\services.ghostship.dk\deepdive");
        public static string PathFileFreeBeer1 = Path.Combine(PathFolderApache24, @"htdocs\services.ghostship.dk\cGoalStateTime");
        public static string PathFileEvent1 = Path.Combine(PathFolderApache24, @"htdocs\drg.ghostship.dk\events\events");
        public static string PathFileEvent2 = Path.Combine(PathFolderApache24, @"htdocs\services.ghostship.dk\events");
        public static string PathFileAssignment1 = Path.Combine(PathFolderApache24, @"htdocs\drg.ghostship.dk\events\weekly");
        public static string PathFileAssignment2 = Path.Combine(PathFolderApache24, @"htdocs\services.ghostship.dk\weekly");
        // GolbergSteamEmulator path files.
        public static string PathFileColdClientLoader = Path.Combine(PathFolderGoldbergSteamEmulator, @"ColdClientLoader.ini");
        public static string PathFileGSEPlayerID = Path.Combine(PathFolderGoldbergSteamEmulator, @"steam_settings\settings\user_steam_id.txt");
        public static string PathFileGSEPlayerName = Path.Combine(PathFolderGoldbergSteamEmulator, @"steam_settings\settings\account_name.txt");
        public static string PathFileSteamClientLoader = Path.Combine(PathFolderGoldbergSteamEmulator, @"steamclient_loader.exe");
        // Windows main path.
        public static string PathFolderWindows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        // Windows path folders.
        public static string PathFolderWebsiteRedirects = Path.Combine(PathFolderWindows, @"System32\drivers\etc");
        // Windows path files.
        public static string PathFileWebsiteRedirects = Path.Combine(PathFolderWindows, @"System32\drivers\etc\hosts");
        // Data required for app functionality.
        public static DataEnum VariableEnums = new DataEnum();
        public static Data Data = new Data();
        // Settings.
        public static DataSettings SettingsDefault = new DataSettings();
        public static DataSettings SettingsCurrent = new DataSettings();
        // Files.
        public static FileINI FileSettings = new FileINI();
        public static FileINI FileSettingsGSE = new FileINI(PathFileColdClientLoader);
        // Timers.
        System.Windows.Threading.DispatcherTimer TimerServicesStatus = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerServicesStatusRedirects = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerServicesStatusCertificates = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerServicesStatusServer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerSearchGameVersion = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerSearchDeepDive = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerSearchEvent = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerWindowPosition = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer TimerWindowSize = new System.Windows.Threading.DispatcherTimer();
        
        ObservableCollection<ItemGameVersion> ItemsGameVersion = new ObservableCollection<ItemGameVersion>();
        ObservableCollection<ItemDeepDive> ItemsDeepDive = new ObservableCollection<ItemDeepDive>();
        ObservableCollection<ItemEvent> ItemsEvent = new ObservableCollection<ItemEvent>();
        ObservableCollection<ItemEventItem> ItemsEventItem = new ObservableCollection<ItemEventItem>();


        // Data.
        public void Data_All_Load()
        {
            Data_GameVersions_Load();
            //
            Enums_Anomalies_Load();
            Enums_Missions_Load();
            Enums_Regions_Load();
            Enums_SideObjectives_Load();
            Enums_Warnings_Load();
            Enums_Color_Load();
            Data_DeepDives_Load();
            //
            Data_Events_Load();
        }
        // Game Versions.
        public void Data_GameVersions_Load()
        {
            // Clear list, because it may be reloaded later.
            Data.GameVersions = new List<DataGameVersion>();
            //
            string[] filePaths = Directory.GetFiles(PathFolderDataGameVersions, "*.ini", SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath); // Get file name without extention.
                // !!! Make a proper check for file name.
                if (fileName != "")
                {
                    FileINI newFileINI = new FileINI(filePath);
                    // Define new game version.
                    DataGameVersion newDataGameVersion = new DataGameVersion();
                    newDataGameVersion.IdFake = fileName;
                    newDataGameVersion.Name = newFileINI.Read("Name", "GameVersion");
                    newDataGameVersion.Path = newFileINI.Read("Path", "GameVersion");
                    newDataGameVersion.Manifest = newFileINI.Read("Manifest", "GameVersion");
                    bool colorRCheck = newFileINI.ReadByte("ColorR", out Byte colorR, "GameVersion");
                    bool colorGCheck = newFileINI.ReadByte("ColorG", out Byte colorG, "GameVersion");
                    bool colorBCheck = newFileINI.ReadByte("ColorB", out Byte colorB, "GameVersion");
                    if (colorRCheck == false || colorGCheck == false || colorBCheck == false)
                    {
                        colorR = 105;
                        colorG = 105;
                        colorB = 105;
                    }
                    newDataGameVersion.Brush = new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB));
                    // Add new game version to the list.
                    Data.GameVersions.Add(newDataGameVersion);
                }
            }
        }
        public List<DataGameVersion> Data_GameVersions_Sort(List<DataGameVersion> inputGameVersions)
        {
            // !!! Split string and reorder.
            List<DataGameVersion> newGameVersions = inputGameVersions.OrderByDescending(x => x.IdFake).ToList();
            return newGameVersions;
        }
        public List<DataGameVersion> Data_GameVersions_Find_GameVersions(string inputText)
        {
            List<DataGameVersion> gameVersions = new List<DataGameVersion>();
            for (int key = 0; key < Data.GameVersions.Count; key++)
            {
                if (
                    Data.GameVersions[key].IdFake.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                    ||
                    Data.GameVersions[key].Name.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                )
                {
                    gameVersions.Add(Data.GameVersions[key]);
                }
            }
            return gameVersions;
        }
        public int Data_GameVersions_Get_IdReal(List<DataGameVersion> inputGameVersions, string inputIdFake)
        {
            int idReal = -1;
            for (int key = 0; key < inputGameVersions.Count; key++)
            {
                if (inputGameVersions[key].IdFake == inputIdFake)
                {
                    idReal = key;
                    break;
                }
            }
            return idReal;
        }
        public DataGameVersion Data_GameVersions_Get_GameVersion(int inputIdReal)
        {
            DataGameVersion DataGameVersion = new DataGameVersion();
            if (inputIdReal >= 0)
            {
                DataGameVersion = Data.GameVersions[inputIdReal];
            }
            return DataGameVersion;
        }
        public void Data_GameVersions_Save_Path(int inputIdReal, string inputPath)
        {
            if (inputIdReal >= 0)
            {
                Data.GameVersions[inputIdReal].Path = inputPath;
                //
                string filePath = Path.Combine(PathFolderDataGameVersions, Data.GameVersions[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("Path", inputPath, "GameVersion");
            }
        }
        public void Data_GameVersions_Save_Name(int inputIdReal, string inputName)
        {
            if (inputIdReal >= 0)
            {
                Data.GameVersions[inputIdReal].Name = inputName;
                //
                string filePath = Path.Combine(PathFolderDataGameVersions, Data.GameVersions[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("Name", inputName, "GameVersion");
            }
        }
        public void Data_GameVersions_Save_Manifest(int inputIdReal, string inputManifest)
        {
            if (inputIdReal >= 0)
            {
                Data.GameVersions[inputIdReal].Manifest = inputManifest;
                //
                string filePath = Path.Combine(PathFolderDataGameVersions, Data.GameVersions[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("Manifest", inputManifest, "GameVersion");
            }
        }
        public void Data_GameVersions_Save_Color(int inputIdReal, Brush inputBrush)
        {
            if (inputIdReal >= 0)
            {
                Data.GameVersions[inputIdReal].Brush = inputBrush;
                SolidColorBrush brushFixed = inputBrush as SolidColorBrush;

                //
                string filePath = Path.Combine(PathFolderDataGameVersions, Data.GameVersions[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("ColorR", brushFixed.Color.R.ToString(), "GameVersion");
                newFileINI.Write("ColorG", brushFixed.Color.G.ToString(), "GameVersion");
                newFileINI.Write("ColorB", brushFixed.Color.B.ToString(), "GameVersion");
            }
        }
        // Deep Dives.
        public void Data_DeepDives_Load()
        {
            // Clear list, because it may be reloaded later.
            Data.DeepDives = new List<DataDeepDive>();
            //
            string[] pathFiles = Directory.GetFiles(PathFolderDataDeepDivesStandard, "*.ini", SearchOption.TopDirectoryOnly);
            foreach (string pathFile in pathFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                // Check if file name is a number and bigger then 0.
                //if (uint.TryParse(fileName.Split('-')[0], out uint fileNameChecked) == true && fileNameChecked > 0)
                if (fileName != "0")
                {
                    FileINI newFileINI = new FileINI(pathFile);
                    // Define new deep dive.
                    DataDeepDive newDataDeepDive = new DataDeepDive();
                    newDataDeepDive.IdFake = fileName;
                    if (newFileINI.ReadUInt("Seed", out uint seedChecked, "DeepDive") == true)
                    {
                        newDataDeepDive.Seed = seedChecked;
                    }
                    newDataDeepDive.GameVersion = newFileINI.Read("GameVersion", "DeepDive");
                    foreach (DataEnumBrush dataEnumBrush in VariableEnums.Brushes)
                    {
                        if (newDataDeepDive.GameVersion == dataEnumBrush.Name)
                        {
                            newDataDeepDive.Brush = dataEnumBrush.Brush;
                            break;
                        }
                    }
                    newDataDeepDive.Event = newFileINI.Read("Event", "DeepDive");
                    newDataDeepDive.Date = newFileINI.Read("Date", "DeepDive");
                    newDataDeepDive.Normal.Tag = newFileINI.Read("NormalTag", "DeepDive");
                    newDataDeepDive.Normal.Name = newFileINI.Read("NormalName", "DeepDive");
                    newDataDeepDive.Normal.Region = newFileINI.Read("NormalRegion", "DeepDive");
                    newDataDeepDive.Normal.Missions[0].Type = newFileINI.Read("NormalMissionType1", "DeepDive");
                    newDataDeepDive.Normal.Missions[1].Type = newFileINI.Read("NormalMissionType2", "DeepDive");
                    newDataDeepDive.Normal.Missions[2].Type = newFileINI.Read("NormalMissionType3", "DeepDive");
                    newDataDeepDive.Normal.Missions[0].Value = newFileINI.Read("NormalMissionValue1", "DeepDive");
                    newDataDeepDive.Normal.Missions[1].Value = newFileINI.Read("NormalMissionValue2", "DeepDive");
                    newDataDeepDive.Normal.Missions[2].Value = newFileINI.Read("NormalMissionValue3", "DeepDive");
                    newDataDeepDive.Normal.Objectives[0].Type = newFileINI.Read("NormalObjectiveType1", "DeepDive");
                    newDataDeepDive.Normal.Objectives[1].Type = newFileINI.Read("NormalObjectiveType2", "DeepDive");
                    newDataDeepDive.Normal.Objectives[2].Type = newFileINI.Read("NormalObjectiveType3", "DeepDive");
                    newDataDeepDive.Normal.Objectives[0].Value = newFileINI.Read("NormalObjectiveValue1", "DeepDive");
                    newDataDeepDive.Normal.Objectives[1].Value = newFileINI.Read("NormalObjectiveValue2", "DeepDive");
                    newDataDeepDive.Normal.Objectives[2].Value = newFileINI.Read("NormalObjectiveValue3", "DeepDive");
                    newDataDeepDive.Normal.Warnings[0] = newFileINI.Read("NormalWarning1", "DeepDive");
                    newDataDeepDive.Normal.Warnings[1] = newFileINI.Read("NormalWarning2", "DeepDive");
                    newDataDeepDive.Normal.Warnings[2] = newFileINI.Read("NormalWarning3", "DeepDive");
                    newDataDeepDive.Normal.Anomalies[0] = newFileINI.Read("NormalAnomaly1", "DeepDive");
                    newDataDeepDive.Normal.Anomalies[1] = newFileINI.Read("NormalAnomaly2", "DeepDive");
                    newDataDeepDive.Normal.Anomalies[2] = newFileINI.Read("NormalAnomaly3", "DeepDive");
                    //
                    newDataDeepDive.Elite.Tag = newFileINI.Read("EliteTag", "DeepDive");
                    newDataDeepDive.Elite.Name = newFileINI.Read("EliteName", "DeepDive");
                    newDataDeepDive.Elite.Region = newFileINI.Read("EliteRegion", "DeepDive");
                    newDataDeepDive.Elite.Missions[0].Type = newFileINI.Read("EliteMissionType1", "DeepDive");
                    newDataDeepDive.Elite.Missions[1].Type = newFileINI.Read("EliteMissionType2", "DeepDive");
                    newDataDeepDive.Elite.Missions[2].Type = newFileINI.Read("EliteMissionType3", "DeepDive");
                    newDataDeepDive.Elite.Missions[0].Value = newFileINI.Read("EliteMissionValue1", "DeepDive");
                    newDataDeepDive.Elite.Missions[1].Value = newFileINI.Read("EliteMissionValue2", "DeepDive");
                    newDataDeepDive.Elite.Missions[2].Value = newFileINI.Read("EliteMissionValue3", "DeepDive");
                    newDataDeepDive.Elite.Objectives[0].Type = newFileINI.Read("EliteObjectiveType1", "DeepDive");
                    newDataDeepDive.Elite.Objectives[1].Type = newFileINI.Read("EliteObjectiveType2", "DeepDive");
                    newDataDeepDive.Elite.Objectives[2].Type = newFileINI.Read("EliteObjectiveType3", "DeepDive");
                    newDataDeepDive.Elite.Objectives[0].Value = newFileINI.Read("EliteObjectiveValue1", "DeepDive");
                    newDataDeepDive.Elite.Objectives[1].Value = newFileINI.Read("EliteObjectiveValue2", "DeepDive");
                    newDataDeepDive.Elite.Objectives[2].Value = newFileINI.Read("EliteObjectiveValue3", "DeepDive");
                    newDataDeepDive.Elite.Warnings[0] = newFileINI.Read("EliteWarning1", "DeepDive");
                    newDataDeepDive.Elite.Warnings[1] = newFileINI.Read("EliteWarning2", "DeepDive");
                    newDataDeepDive.Elite.Warnings[2] = newFileINI.Read("EliteWarning3", "DeepDive");
                    newDataDeepDive.Elite.Anomalies[0] = newFileINI.Read("EliteAnomaly1", "DeepDive");
                    newDataDeepDive.Elite.Anomalies[1] = newFileINI.Read("EliteAnomaly2", "DeepDive");
                    newDataDeepDive.Elite.Anomalies[2] = newFileINI.Read("EliteAnomaly3", "DeepDive");
                    // Add new deep dive to the list.
                    Data.DeepDives.Add(newDataDeepDive);
                }
            }
        }
        public List<DataDeepDive> Data_DeepDives_Sort(List<DataDeepDive> inputDeepDives)
        {
            List<DataDeepDive> newDeepDives = inputDeepDives.OrderByDescending(
                x => int.Parse(x.IdFake.Split('-')[0])
            ).ThenByDescending(x => x.IdFake, new CustomStringComparer()
            ).ToList();
            return newDeepDives;
        }
        class CustomStringComparer : IComparer<String>
        {
            public int Compare(string? x, string? y)
            {
                string x1 = "";
                if (x.Split('-').Length > 1)
                {
                    x1 = x.Split('-')[1];
                }
                string y1 = "";
                if (y.Split('-').Length > 1)
                {
                    y1 = y.Split('-')[1];
                }
                return string.Compare(x1, y1);
            }
        }
        public List<DataDeepDive> Data_DeepDives_Find_DeepDives(string inputText)
        {
            List<DataDeepDive> deepDives = new List<DataDeepDive>();
            for (int key = 0; key < Data.DeepDives.Count; key++)
            {
                if (Data.DeepDives[key].Seed != 0 || Data.DeepDives[key].Seed == 0 && SettingsCurrent.DeepDive.LostDeepDives == true)
                {
                    // !!! Add other ways to find deep dive.
                    if (
                        Data.DeepDives[key].IdFake.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Seed.ToString().Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].GameVersion.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Tag.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Tag.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Name.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Name.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Missions[0].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Missions[1].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Missions[2].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Missions[0].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Missions[1].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Missions[2].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Objectives[0].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Objectives[1].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Objectives[2].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Objectives[0].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Objectives[1].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Objectives[2].Type.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Warnings[0].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Warnings[1].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Warnings[2].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Warnings[0].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Warnings[1].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Warnings[2].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Anomalies[0].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Anomalies[1].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Normal.Anomalies[2].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Anomalies[0].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Anomalies[1].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.DeepDives[key].Elite.Anomalies[2].Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                    )
                    {
                        deepDives.Add(Data.DeepDives[key]);
                    }
                }
            }
            return deepDives;
        }
        public int Data_DeepDives_Get_IdReal(List<DataDeepDive> inputDeepDives, string inputIdFake)
        {
            int idReal = -1;
            for (int key = 0; key < inputDeepDives.Count; key++)
            {
                if (inputDeepDives[key].IdFake == inputIdFake)
                {
                    idReal = key;
                    break;
                }
            }
            return idReal;
        }
        public DataDeepDive Data_DeepDives_Get_DeepDive(int inputIdReal)
        {
            DataDeepDive dataDeepDive = new DataDeepDive();
            if (inputIdReal >= 0)
            {
                dataDeepDive = Data.DeepDives[inputIdReal];
            }
            return dataDeepDive;
        }
        public void Data_DeepDives_Save_TagNormal(int inputIdReal, string inputTagNormal)
        {
            if (inputIdReal >= 0)
            {
                Data.DeepDives[inputIdReal].Normal.Tag = inputTagNormal;
                //
                string filePath = Path.Combine(PathFolderDataDeepDivesStandard, Data.DeepDives[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("NormalTag", inputTagNormal, "DeepDive");
            }
        }
        public void Data_DeepDives_Save_TagElite(int inputIdReal, string inputTagElite)
        {
            if (inputIdReal >= 0)
            {
                Data.DeepDives[inputIdReal].Elite.Tag = inputTagElite;
                //
                string filePath = Path.Combine(PathFolderDataDeepDivesStandard, Data.DeepDives[inputIdReal].IdFake + ".ini");
                FileINI newFileINI = new FileINI(filePath);
                newFileINI.Write("EliteTag", inputTagElite, "DeepDive");
            }
        }
        // Events.
        public void Data_Events_Load()
        {
            // Clear list, because it may be reloaded later.
            Data.Events = new List<DataEvent>();
            //
            string[] pathFiles = Directory.GetFiles(PathFolderDataEvents, "*.ini", SearchOption.TopDirectoryOnly);
            foreach (string pathFile in pathFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                // Check if file name is a number and bigger then 0.
                if (uint.TryParse(fileName, out uint fileNameChecked) == true && fileNameChecked > 0)
                {
                    FileINI newFileINI = new FileINI(pathFile);
                    // Define new event.
                    DataEvent newDataEvent = new DataEvent();
                    newDataEvent.IdFake = fileName;
                    string year = newFileINI.Read("Year", "Event");
                    if (uint.TryParse(year, out uint yearChecked) == true)
                    {
                        newDataEvent.Year = year;
                    }
                    else
                    {
                        newDataEvent.Year = "2018";
                    }
                    newDataEvent.Name = newFileINI.Read("Name", "Event");
                    newDataEvent.Command = newFileINI.Read("Command", "Event");
                    bool check = true;
                    int count = 1;
                    while (check == true)
                    {
                        string itemName = newFileINI.Read("ItemName" + count, "Event");
                        // Check if item name is not empty.
                        if (itemName != "")
                        {
                            string itemType = newFileINI.Read("ItemType" + count, "Event");
                            // Check if item type is not empty.
                            if (itemType != "")
                            {
                                DataEventItem item = new DataEventItem();
                                item.Name = itemName;
                                item.Type = itemType;
                                //
                                newDataEvent.Items.Add(item);
                                //
                                count = count + 1;
                            }
                            else
                            {
                                // Turn loop off.
                                check = false;
                            }
                        }
                        else
                        {
                            // Turn loop off.
                            check = false;
                        }

                    }
                    // Add new DataEvent to the list.
                    Data.Events.Add(newDataEvent);
                }
            }
        }
        public List<DataEvent> Data_Events_Sort(List<DataEvent> inputEvents)
        {
            List<DataEvent> newEvents = inputEvents.OrderByDescending(x => int.Parse(x.IdFake)).ToList();
            return newEvents;
        }
        public List<DataEvent> Data_Events_Find_Events(string inputText)
        {
            List<DataEvent> events = new List<DataEvent>();
            for (int key = 0; key < Data.Events.Count; key++)
            {
                if (Data.Events[key].Command != "" || Data.Events[key].Command == "" && SettingsCurrent.Event.LostEvents == true)
                {
                    // !!! Add other ways to find event.
                    if (
                        Data.Events[key].IdFake.ToString().Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.Events[key].Year.ToString().Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                        ||
                        Data.Events[key].Name.Contains(inputText, StringComparison.CurrentCultureIgnoreCase) == true
                    )
                    {
                        events.Add(Data.Events[key]);
                    }
                }
            }
            return events;
        }
        public int Data_Events_Get_IdReal(List<DataEvent> inputEvents, string inputIdFake)
        {
            int idReal = -1;
            for (int key = 0; key < inputEvents.Count; key++)
            {
                if (inputEvents[key].IdFake == inputIdFake)
                {
                    idReal = key;
                    break;
                }
            }
            return idReal;
        }
        public DataEvent Data_Events_Get_Event(int inputIdReal)
        {
            DataEvent dataEvent = new DataEvent();
            if (inputIdReal >= 0)
            {
                dataEvent = Data.Events[inputIdReal];
            }
            return dataEvent;
        }
        // Enums.
        public void Enums_Anomalies_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.Anomalies = new List<string>();
            // Add epmty string.
            VariableEnums.Anomalies.Add("");
            // Add loaded strings from data.
            string[] lines = File.ReadAllLines(PathFileEnumAnomalies);
            foreach (string line in lines)
            {
                VariableEnums.Anomalies.Add(line);
            }
        }
        public void Enums_Missions_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.Missions = new List<string>();
            // Add epmty string.
            VariableEnums.Missions.Add("");
            // Add loaded strings from data.
            string[] lines = File.ReadAllLines(PathFileEnumMissions);
            foreach (string line in lines)
            {
                VariableEnums.Missions.Add(line);
            }
        }
        public void Enums_Regions_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.Regions = new List<string>();
            // Add epmty string.
            VariableEnums.Regions.Add("");
            // Add loaded strings from data.
            string[] lines = File.ReadAllLines(PathFileEnumRegions);
            foreach (string line in lines)
            {
                VariableEnums.Regions.Add(line);
            }
        }
        public void Enums_SideObjectives_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.SideObjectives = new List<string>();
            // Add epmty string.
            VariableEnums.SideObjectives.Add("");
            // Add loaded strings from data.
            string[] lines = File.ReadAllLines(PathFileEnumSideObjectives);
            foreach (string line in lines)
            {
                VariableEnums.SideObjectives.Add(line);
            }
        }
        public void Enums_Warnings_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.Warnings = new List<string>();
            // Add epmty string.
            VariableEnums.Warnings.Add("");
            // Add loaded strings from data.
            string[] lines = File.ReadAllLines(PathFileEnumWarnings);
            foreach (string line in lines)
            {
                VariableEnums.Warnings.Add(line);
            }
        }
        public void Enums_Color_Load()
        {
            // Clear list, because it may be reloaded later.
            VariableEnums.Brushes = new List<DataEnumBrush>();
            //
            foreach (DataGameVersion dataGameVersion in Data.GameVersions)
            {
                DataEnumBrush newDataEnumBrush = new DataEnumBrush();
                newDataEnumBrush.Name = dataGameVersion.IdFake;
                newDataEnumBrush.Brush = dataGameVersion.Brush;
                VariableEnums.Brushes.Add(newDataEnumBrush);
            }
        }



        // Settings.
        public void Settings_All_Load()
        {
            Settings_Services_IP_Load();
            Settings_Services_ChangeRedirects_Load();
            Settings_Services_StartServer_Load();
            //
            Settings_Game_Path_Load();
            Settings_Game_PlayerId_Load();
            Settings_Game_PlayerName_Load();
            Settings_Game_Command_Load();
            Settings_Game_SelectedId_Load();
            //
            Settings_DeepDive_Seed_Load();
            Settings_DeepDive_LostDeepDives_Load();
            Settings_DeepDive_SelectedId_Load();
            //
            Settings_Event_Command_Load();
            Settings_Event_FreeBeers_Load();
            Settings_Event_LostEvents_Load();
            Settings_Event_SelectedId_Load();
            //
            Settings_Assignment_Seed_Load();
            //
            Settings_Settings_PositionX_Load();
            Settings_Settings_PositionY_Load();
            Settings_Settings_SizeX_Load();
            Settings_Settings_SizeY_Load();
            Settings_Settings_HideNotes_Load();
        }
        // Services.
        public void Settings_Services_IP_Load()
        {
            string ip = FileSettings.Read("IP", "Services");
            if (ip != "")
            {
                SettingsCurrent.Services.IP = ip;
            }
            else
            {
                Settings_Services_IP_Save(SettingsDefault.Services.IP);
            }
        }
        public void Settings_Services_IP_Save(string inputIP)
        {
            // Internal Setting.
            SettingsCurrent.Services.IP = inputIP;
            FileSettings.Write("IP", inputIP, "Services");
        }
        public void Settings_Services_ChangeRedirects_Load()
        {
            if (FileSettings.ReadBool("ChangeRedirects", out bool setting, "Services") == true)
            {
                SettingsCurrent.Services.ChangeRedirects = setting;
            }
            else
            {
                Settings_Services_ChangeRedirects_Save(SettingsDefault.Services.ChangeRedirects);
            }
        }
        public void Settings_Services_ChangeRedirects_Save(bool inputSetting)
        {
            // Internal Setting.
            SettingsCurrent.Services.ChangeRedirects = inputSetting;
            FileSettings.Write("ChangeRedirects", inputSetting.ToString(), "Services");
        }
        public void Settings_Services_StartServer_Load()
        {
            if (FileSettings.ReadBool("StartServer", out bool setting, "Services") == true)
            {
                SettingsCurrent.Services.StartServer = setting;
            }
            else
            {
                Settings_Services_StartServer_Save(SettingsDefault.Services.StartServer);
            }
        }
        public void Settings_Services_StartServer_Save(bool setting)
        {
            // Internal Setting.
            SettingsCurrent.Services.StartServer = setting;
            FileSettings.Write("StartServer", setting.ToString(), "Services");
        }
        // Game.
        public void Settings_Game_Path_Load()
        {
            string path = FileSettings.Read("Path", "Game");
            if (path != "")
            {
                SettingsCurrent.Game.Path = path;
            }
            else
            {
                Settings_Game_Path_Save(SettingsDefault.Game.Path);
            }
        }
        public void Settings_Game_Path_Save(string inputPath)
        {
            // Internal Setting.
            SettingsCurrent.Game.Path = inputPath;
            FileSettings.Write("Path", inputPath, "Game");
        }
        public void Settings_Game_PlayerId_Load()
        {
            string[] lines = File.ReadAllLines(PathFileGSEPlayerID);
            // If file will be empty, it's length will be null and it will not load empty string.
            if (lines.Length > 0)
            {
                SettingsCurrent.Game.PlayerId = lines[0];
            }
            else
            {
                Settings_Game_PlayerId_Save(SettingsDefault.Game.PlayerId);
            }
        }
        public void Settings_Game_PlayerId_Save(string inputPlayerId)
        {
            // Internal Setting.
            SettingsCurrent.Game.PlayerId = inputPlayerId;
            // External Setting.
            File.WriteAllText(PathFileGSEPlayerID, inputPlayerId);
        }
        public void Settings_Game_PlayerName_Load()
        {
            string[] lines = File.ReadAllLines(PathFileGSEPlayerName);
            // If file will be empty, it's length will be null and it will not load empty string.
            if (lines.Length > 0)
            {
                SettingsCurrent.Game.PlayerName = lines[0];
            }
            else
            {
                Settings_Game_PlayerName_Save(SettingsDefault.Game.PlayerName);
            }
        }
        public void Settings_Game_PlayerName_Save(string inputPlayerName)
        {
            // Internal Setting.
            SettingsCurrent.Game.PlayerName = inputPlayerName;
            // External Setting.
            File.WriteAllText(PathFileGSEPlayerName, inputPlayerName);
        }
        public void Settings_Game_Command_Load()
        {
            string command = FileSettingsGSE.Read("ExeCommandLine", "SteamClient");
            if (command != "")
            {
                SettingsCurrent.Game.Command = command;
            }
            else
            {
                Settings_Game_Command_Save(SettingsDefault.Game.Command);
            }
        }
        public void Settings_Game_Command_Save(string inputCommand)
        {
            // Internal Setting.
            SettingsCurrent.Game.Command = inputCommand;
            // External Setting.
            FileSettingsGSE.Write("ExeCommandLine", inputCommand, "SteamClient");
        }
        public void Settings_Game_SelectedId_Load()
        {
            string selectedIdFake = FileSettings.Read("SelectedId", "Game");
            if (selectedIdFake != "")
            {
                int selectedIdReal = Data_GameVersions_Get_IdReal(Data.GameVersions, selectedIdFake);
                if (selectedIdReal == -1)
                {
                    Settings_Game_SelectedId_Save(SettingsDefault.Game.SelectedIdReal, SettingsDefault.Game.SelectedIdFake);
                }
                else
                {
                    SettingsCurrent.Game.SelectedIdReal = selectedIdReal;
                    SettingsCurrent.Game.SelectedIdFake = selectedIdFake;
                }
            }
            else
            {
                Settings_Game_SelectedId_Save(SettingsDefault.Game.SelectedIdReal, SettingsDefault.Game.SelectedIdFake);
            }
        }
        public void Settings_Game_SelectedId_Save(int inputSelectedIdReal, string inputSelectedIdFake)
        {
            // Internal Setting.
            SettingsCurrent.Game.SelectedIdReal = inputSelectedIdReal;
            SettingsCurrent.Game.SelectedIdFake = inputSelectedIdFake;
            FileSettings.Write("SelectedId", inputSelectedIdFake, "Game");
        }
        public void Settings_Game_Search_Save(string inputSearch)
        {
            // Internal Setting.
            SettingsCurrent.Game.Search = inputSearch;
        }
        // Deep Dive.
        public void Settings_DeepDive_Seed_Load()
        {
            if (FileSettings.ReadUInt("Seed", out uint seedChecked, "DeepDive") == true)
            {
                SettingsCurrent.DeepDive.Seed = seedChecked;
            }
            else
            {
                Settings_DeepDive_Seed_Save(SettingsDefault.DeepDive.Seed);
            }
        }
        public void Settings_DeepDive_Seed_Save(uint inputSeed)
        {
            // Internal Setting.
            SettingsCurrent.DeepDive.Seed = inputSeed;
            FileSettings.Write("Seed", inputSeed.ToString(), "DeepDive");
            // External Setting.
            string command = "{\"Seed\":" + inputSeed + ",\"SeedV2\":" + inputSeed + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
            File.WriteAllText(PathFileDeepDive1, command);
            File.WriteAllText(PathFileDeepDive2, command);
        }
        public void Settings_DeepDive_LostDeepDives_Load()
        {
            if (FileSettings.ReadBool("LostDeepDives", out bool lostDeepDivesChecked, "DeepDive") == true)
            {
                SettingsCurrent.DeepDive.LostDeepDives = lostDeepDivesChecked;
            }
            else
            {
                Settings_DeepDive_LostDeepDives_Save(SettingsDefault.DeepDive.LostDeepDives);
            }
        }
        public void Settings_DeepDive_LostDeepDives_Save(bool inputLostDeepDives)
        {
            // Internal Setting.
            SettingsCurrent.DeepDive.LostDeepDives = inputLostDeepDives;
            FileSettings.Write("LostDeepDives", inputLostDeepDives.ToString(), "DeepDive");
        }
        public void Settings_DeepDive_SelectedId_Load()
        {
            string selectedIdFake = FileSettings.Read("SelectedId", "DeepDive");
            if (selectedIdFake != "")
            {
                int selectedIdReal = Data_DeepDives_Get_IdReal(Data.DeepDives, selectedIdFake);
                if (selectedIdReal == -1)
                {
                    Settings_DeepDive_SelectedId_Save(SettingsDefault.DeepDive.SelectedIdReal, SettingsDefault.DeepDive.SelectedIdFake);
                }
                else
                {
                    SettingsCurrent.DeepDive.SelectedIdReal = selectedIdReal;
                    SettingsCurrent.DeepDive.SelectedIdFake = selectedIdFake;
                }
            }
            else
            {
                Settings_DeepDive_SelectedId_Save(SettingsDefault.DeepDive.SelectedIdReal, SettingsDefault.DeepDive.SelectedIdFake);
            }
        }
        public void Settings_DeepDive_SelectedId_Save(int inputSelectedIdReal, string inputSelectedIdFake)
        {
            // Internal Setting.
            SettingsCurrent.DeepDive.SelectedIdReal = inputSelectedIdReal;
            SettingsCurrent.DeepDive.SelectedIdFake = inputSelectedIdFake;
            FileSettings.Write("SelectedId", inputSelectedIdFake, "DeepDive");
        }
        public void Settings_DeepDive_Search_Save(string inputSearch)
        {
            // Internal Setting.
            SettingsCurrent.DeepDive.Search = inputSearch;
        }
        // Event.
        public void Settings_Event_Command_Load()
        {
            string command = FileSettings.Read("Command", "Event");
            if (command != "")
            {
                SettingsCurrent.Event.Command = command;
            }
            else
            {
                Settings_Event_Command_Save(SettingsDefault.Event.Command);
            }
        }
        public void Settings_Event_Command_Save(string inputCommand)
        {
            // Internal Setting.
            SettingsCurrent.Event.Command = inputCommand;
            FileSettings.Write("Command", inputCommand, "Event");
            // External Setting.
            string command = "{\"ActiveEvents\":[\"" + Regex.Replace(inputCommand, @" ", "").Replace(",", "\",\"") + "\"]}"; // Remove spaces and replace "comma" with "quotes,comma,qoutes".
            File.WriteAllText(PathFileEvent1, command);
            File.WriteAllText(PathFileEvent2, command);
        }
        public void Settings_Event_FreeBeers_Load()
        {
            if (FileSettings.ReadBool("FreeBeers", out bool freeBeerChecked, "Event") == true)
            {
                SettingsCurrent.Event.FreeBeers = freeBeerChecked;
            }
            else
            {
                Settings_Event_FreeBeers_Save(SettingsDefault.Event.FreeBeers);
            }
        }
        public void Settings_Event_FreeBeers_Save(bool inputFreeBeers)
        {
            // Internal Setting.
            SettingsCurrent.Event.FreeBeers = inputFreeBeers;
            FileSettings.Write("FreeBeers", inputFreeBeers.ToString(), "Event");
            // External Setting.
            string command = "{\"FreeBeers\":" + inputFreeBeers + "}";
            File.WriteAllText(PathFileFreeBeer1, command);
        }
        public void Settings_Event_LostEvents_Load()
        {
            if (FileSettings.ReadBool("LostEvents", out bool lostEventsChecked, "Event") == true)
            {
                SettingsCurrent.Event.LostEvents = lostEventsChecked;
            }
            else
            {
                Settings_Event_LostEvents_Save(SettingsDefault.Event.LostEvents);
            }
        }
        public void Settings_Event_LostEvents_Save(bool inputLostEvents)
        {
            // Internal Setting.
            SettingsCurrent.Event.LostEvents = inputLostEvents;
            FileSettings.Write("LostEvents", inputLostEvents.ToString(), "Event");
        }
        public void Settings_Event_SelectedId_Load()
        {
            string selectedIdFake = FileSettings.Read("SelectedId", "Event");
            if (selectedIdFake != "")
            {
                int selectedIdReal = Data_Events_Get_IdReal(Data.Events, selectedIdFake);
                if (selectedIdReal == -1)
                {
                    Settings_Event_SelectedId_Save(SettingsDefault.Event.SelectedIdReal, SettingsDefault.Event.SelectedIdFake);
                }
                else
                {
                    SettingsCurrent.Event.SelectedIdReal = selectedIdReal;
                    SettingsCurrent.Event.SelectedIdFake = selectedIdFake;
                }
            }
            else
            {
                Settings_Event_SelectedId_Save(SettingsDefault.Event.SelectedIdReal, SettingsDefault.Event.SelectedIdFake);
            }
        }
        public void Settings_Event_SelectedId_Save(int inputSelectedIdReal, string inputSelectedIdFake)
        {
            // Internal Setting.
            SettingsCurrent.Event.SelectedIdReal = inputSelectedIdReal;
            SettingsCurrent.Event.SelectedIdFake = inputSelectedIdFake;
            FileSettings.Write("SelectedId", inputSelectedIdFake, "Event");
        }
        public void Settings_Event_Search_Save(string inputSearch)
        {
            // Internal Setting.
            SettingsCurrent.Event.Search = inputSearch;
        }
        // Assignment.
        public void Settings_Assignment_Seed_Load()
        {
            if (FileSettings.ReadUInt("Seed", out uint seedChecked, "Assignment") == true)
            {
                SettingsCurrent.Assignment.Seed = seedChecked;
            }
            else
            {
                Settings_Assignment_Seed_Save(SettingsDefault.Assignment.Seed);
            }
        }
        public void Settings_Assignment_Seed_Save(uint inputSeed)
        {
            // Internal Setting.
            SettingsCurrent.Assignment.Seed = inputSeed;
            FileSettings.Write("Seed", inputSeed.ToString(), "Assignment");
            // External Setting.
            string command = "{\"Seed\":" + inputSeed + ",\"ExpirationTime\":\"2100-01-01T00:00:00Z\"}";
            File.WriteAllText(PathFileAssignment1, command);
            File.WriteAllText(PathFileAssignment2, command);
        }
        // Settings.
        public void Settings_Settings_PositionX_Load()
        {
            if (FileSettings.ReadDouble("PositionX", out double positionXChecked, "Settings") == true)
            {
                SettingsCurrent.Settings.PositionX = positionXChecked;
            }
            else
            {
                Settings_Settings_PositionX_Save(SettingsDefault.Settings.PositionX);
            }
        }
        public void Settings_Settings_PositionX_Save(double inputPositionX)
        {
            // Internal Setting.
            SettingsCurrent.Settings.PositionX = inputPositionX;
            FileSettings.Write("PositionX", inputPositionX.ToString(), "Settings");
        }
        public void Settings_Settings_PositionY_Load()
        {
            if (FileSettings.ReadDouble("PositionY", out double positionYChecked, "Settings") == true)
            {
                SettingsCurrent.Settings.PositionY = positionYChecked;
            }
            else
            {
                Settings_Settings_PositionY_Save(SettingsDefault.Settings.PositionY);
            }
        }
        public void Settings_Settings_PositionY_Save(double inputPositionY)
        {
            // Internal Setting.
            SettingsCurrent.Settings.PositionY = inputPositionY;
            FileSettings.Write("PositionY", inputPositionY.ToString(), "Settings");
        }
        public void Settings_Settings_SizeX_Load()
        {
            if (FileSettings.ReadDouble("SizeX", out double sizeXChecked, "Settings") == true)
            {
                SettingsCurrent.Settings.SizeX = sizeXChecked;
            }
            else
            {
                Settings_Settings_SizeX_Save(SettingsDefault.Settings.SizeX);
            }
        }
        public void Settings_Settings_SizeX_Save(double inputSizeX)
        {
            // Internal Setting.
            SettingsCurrent.Settings.SizeX = inputSizeX;
            FileSettings.Write("SizeX", inputSizeX.ToString(), "Settings");
        }
        public void Settings_Settings_SizeY_Load()
        {
            if (FileSettings.ReadDouble("SizeY", out double sizeYChecked, "Settings") == true)
            {
                SettingsCurrent.Settings.SizeY = sizeYChecked;
            }
            else
            {
                Settings_Settings_SizeY_Save(SettingsDefault.Settings.SizeY);
            }
        }
        public void Settings_Settings_SizeY_Save(double inputSizeY)
        {
            // Internal Setting.
            SettingsCurrent.Settings.SizeY = inputSizeY;
            FileSettings.Write("SizeY", inputSizeY.ToString(), "Settings");
        }
        public void Settings_Settings_HideNotes_Load()
        {
            if (FileSettings.ReadBool("HideNotes", out bool hideNotesChecked, "Settings") == true)
            {
                SettingsCurrent.Settings.HideNotes = hideNotesChecked;
            }
            else
            {
                Settings_Settings_HideNotes_Save(SettingsDefault.Settings.HideNotes);
            }
        }
        public void Settings_Settings_HideNotes_Save(bool inputHideNotes)
        {
            // Internal Setting.
            SettingsCurrent.Settings.HideNotes = inputHideNotes;
            FileSettings.Write("HideNotes", inputHideNotes.ToString(), "Settings");
        }



        // Redirects.
        public void Redirects_Add()
        {
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Fix file not having empty line at the end and add lines between redirects.
                if (File.ReadAllText(PathFileWebsiteRedirects).EndsWith("\r\n") == false)
                {
                    // Add two lines, first for not writing redirect on top of existing one, second for space between redirects.
                    File.AppendAllText(PathFileWebsiteRedirects, Environment.NewLine + Environment.NewLine);
                }
                else
                {
                    // Add one line for space between redirects.
                    File.AppendAllText(PathFileWebsiteRedirects, Environment.NewLine);
                }
                // Add label for app redirects.
                File.AppendAllText(PathFileWebsiteRedirects, "#DeepDiveEmulator Redirects" + Environment.NewLine);
                // Add redirects.
                foreach (string pathFile in pathFiles)
                {
                    // Define redirect name.
                    string hostName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.
                    // Define command.
                    string command = SettingsCurrent.Services.IP + " " + hostName;
                    // Write command down at the end of all lines.
                    File.AppendAllText(PathFileWebsiteRedirects, command + Environment.NewLine);
                }
            }

            // !!! Maybe use temp file and overwrite original file with it for windows protection fix?
            /*
            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, lines);
            File.Delete(PathFileWebsiteRedirects);
            File.Move(tempFile, PathFileWebsiteRedirects);
            */
        }
        public void Redirects_Remove()
        {
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                string[] lines = File.ReadAllLines(PathFileWebsiteRedirects);
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
                File.WriteAllLines(PathFileWebsiteRedirects, lines);

                // !!! Maybe use temp file and overwrite original file with it for windows protection fix?
                /*
                var tempFile = Path.GetTempFileName();
                File.WriteAllLines(tempFile, lines);
                File.Delete(PathFileWebsiteRedirects);
                File.Move(tempFile, PathFileWebsiteRedirects);
                */
            }
        }
        public bool Redirects_CheckAdded()
        {
            string redirectsIP = "";
            //
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
            if (pathFiles.Length > 0)
            {
                // Count for the ammount of current redirects with required host name.
                int count = 0;
                //
                foreach (string pathFile in pathFiles)
                {
                    string hostName = Path.GetFileNameWithoutExtension(pathFile); // Get file name without extention.

                    //
                    string[] lines = File.ReadAllLines(PathFileWebsiteRedirects);
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
        public void Redirects_OpenFolder()
        {
            Process.Start("explorer.exe", PathFolderWebsiteRedirects);
        }
        // Certificates.
        public void Certificates_Add()
        {
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
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
        public void Certificates_Remove()
        {
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
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
        public bool Certificates_CheckAdded()
        {
            string[] pathFiles = Directory.GetFiles(PathFolderWebsiteCertificates, "*.crt", SearchOption.TopDirectoryOnly);
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
        public void Certificates_OpenFolder()
        {
            Process.Start("explorer.exe", PathFolderWebsiteCertificates);
        }
        // Server.
        public void Server_Install()
        {
            string pathRequired = PathFolderApache24;
            List<string> tempLines = new List<string>();
            //
            string[] lines = File.ReadAllLines(PathFileHTTPDConfig);
            foreach (string line in lines)
            {
                if (line.Contains("Define SRVROOT", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    string command = "Define SRVROOT \"" + pathRequired + "\"";
                    tempLines.Add(command);
                }
                else
                {
                    tempLines.Add(line);
                }
            }
            lines = tempLines.ToArray();
            File.WriteAllLines(PathFileHTTPDConfig, lines);
        }
        public void Server_Start()
        {
            // Write down required path for server to work.
            Server_Install();
            //
            Process process = new Process();
            process.StartInfo.FileName = PathFileHTTPD;
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
        public bool Server_CheckRunning()
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
        // Game.
        public bool? Game_Path_Check(string inputPath)
        {
            bool? check;
            if (File.Exists(inputPath) == true)
            {
                if (inputPath.EndsWith("FSD-Win64-Shipping.exe") == true)
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
        public void Game_Start(string inputPath)
        {
            string steamClientDLL = Path.Combine(PathFolderGoldbergSteamEmulator, @"steamclient.dll");
            string steamClient64DLL = Path.Combine(PathFolderGoldbergSteamEmulator, @"steamclient64.dll");

            FileSettingsGSE.Write("Exe", inputPath, "SteamClient");
            FileSettingsGSE.Write("SteamClientDll", steamClientDLL, "SteamClient");
            FileSettingsGSE.Write("SteamClient64Dll", steamClient64DLL, "SteamClient");

            Process.Start(PathFileSteamClientLoader);
        }
        // Seed.
        public uint Seed_Generate()
        {
            // Generate value from 0 to 4294967295.
            uint seed = (uint)new Random().Next(-int.MaxValue, int.MaxValue);
            return seed;
        }
        // String.
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



        // Tabs.
        public void Tab_All_Update()
        {
            Tab_Services_Update();
            Tab_Game_Update();
            Tab_DeepDive_Update();
            Tab_Event_Update();
            Tab_Assignment_Update();
            Tab_Settings_Update();
            //
            TimerServicesStatus.Tick += TimerServicesStatus_Tick;
            TimerServicesStatus.Interval = TimeSpan.FromSeconds(10);
            //
            TimerServicesStatus.Tick += TimerServicesStatusRedirects_Tick;
            TimerServicesStatus.Interval = TimeSpan.FromSeconds(2.5);
            //
            TimerServicesStatus.Tick += TimerServicesStatusCertificates_Tick;
            TimerServicesStatus.Interval = TimeSpan.FromSeconds(2.5);
            //
            TimerServicesStatus.Tick += TimerServicesStatusServer_Tick;
            TimerServicesStatus.Interval = TimeSpan.FromSeconds(2.5);
            //
            TimerSearchGameVersion.Tick += TimerSearchGameVersion_Tick;
            TimerSearchGameVersion.Interval = TimeSpan.FromSeconds(0.5);
            //
            TimerSearchDeepDive.Tick += TimerSearchDeepDive_Tick;
            TimerSearchDeepDive.Interval = TimeSpan.FromSeconds(0.5);
            //
            TimerSearchEvent.Tick += TimerSearchEvent_Tick;
            TimerSearchEvent.Interval = TimeSpan.FromSeconds(0.5);
            //
            TimerWindowPosition.Tick += TimerWindowPosition_Tick;
            TimerWindowPosition.Interval = TimeSpan.FromSeconds(0.5);
            //
            TimerWindowSize.Tick += TimerWindowSize_Tick;
            TimerWindowSize.Interval = TimeSpan.FromSeconds(0.5);
        }
        // Tab Services.
        public void Tab_Services_Update()
        {
            bool redsAdded = Redirects_CheckAdded();
            bool certsAdded = Certificates_CheckAdded();
            bool servRunning = Server_CheckRunning();
            //
            TBoxIP.Text = SettingsCurrent.Services.IP;
            CBoxRedsChange.IsChecked = SettingsCurrent.Services.ChangeRedirects;
            Tab_Services_ChangeRedirects_Add(SettingsCurrent.Services.ChangeRedirects);
            CBoxServStart.IsChecked = SettingsCurrent.Services.StartServer;
            Tab_Services_StartServer(SettingsCurrent.Services.StartServer);
            CBoxRedsAdded_Set(redsAdded);
            CBoxCertsAdded_Set(certsAdded);
            CBoxServRunning_Set(servRunning);
            TimerServicesCheck_Start();
        }
        public void Tab_Services_ChangeRedirects_Add(bool inputSetting)
        {
            if (inputSetting == true)
            {
                Redirects_Remove();
                Redirects_Add();
            }
        }
        public void Tab_Services_ChangeRedirects_Remove(bool inputSetting)
        {
            if (inputSetting == true)
            {
                Redirects_Remove();
            }
        }
        public void Tab_Services_StartServer(bool inputSetting)
        {
            if (inputSetting == true)
            {
                Server_Start();
            }
        }
        //
        public void CBoxRedsAdded_Set(bool inputSetting)
        {
            if (inputSetting == true)
            {
                CBoxRedsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxRedsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void CBoxCertsAdded_Set(bool inputSetting)
        {
            if (inputSetting == true)
            {
                CBoxCertsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxCertsAdded.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void CBoxServRunning_Set(bool inputSetting)
        {
            if (inputSetting == true)
            {
                CBoxServRunning.Background = new SolidColorBrush(Color.FromRgb(64, 255, 64));
            }
            else
            {
                CBoxServRunning.Background = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            }
        }
        public void TimerServicesCheck_Start()
        {
            TimerServicesStatus.Start();
        }
        public void TimerRedsAdded_Update()
        {
            TimerRedsAdded_Stop();
            TimerRedsAdded_Start();
        }
        public void TimerRedsAdded_Start()
        {
            TimerServicesStatusRedirects.Start();
        }
        public void TimerRedsAdded_Stop()
        {
            TimerServicesStatusRedirects.Stop();
        }
        public void TimerCertsAdded_Update()
        {
            TimerCertsAdded_Stop();
            TimerCertsAdded_Start();
        }
        public void TimerCertsAdded_Start()
        {
            TimerServicesStatusCertificates.Start();
        }
        public void TimerCertsAdded_Stop()
        {
            TimerServicesStatusCertificates.Stop();
        }
        public void TimerServRunning_Update()
        {
            TimerServRunning_Stop();
            TimerServRunning_Start();
        }
        public void TimerServRunning_Start()
        {
            TimerServicesStatusServer.Start();
        }
        public void TimerServRunning_Stop()
        {
            TimerServicesStatusServer.Stop();
        }
        // Tab Game.
        public void Tab_Game_Update()
        {
            List<DataGameVersion> gameVersions = Data.GameVersions;
            DataGameVersion dataGameVersion = Data_GameVersions_Get_GameVersion(SettingsCurrent.Game.SelectedIdReal);
            //
            if (SettingsCurrent.Game.SelectedIdReal >= 0)
            {
                TBoxPath.Text = dataGameVersion.Path;
                TBoxVerNumber.IsEnabled = false;
                TBoxVerManifest.IsEnabled = false;
            }
            else
            {
                TBoxPath.Text = SettingsCurrent.Game.Path;
                TBoxVerNumber.IsEnabled = true;
                TBoxVerManifest.IsEnabled = true;
            }
            TBoxPlrId.Text = SettingsCurrent.Game.PlayerId;
            TBoxPlrName.Text = SettingsCurrent.Game.PlayerName;
            TBoxLnchParams.Text = SettingsCurrent.Game.Command;
            TBoxVerNumber.Text = dataGameVersion.IdFake;
            TBoxVerName.Text = dataGameVersion.Name;
            TBoxVerManifest.Text = dataGameVersion.Manifest;
            TBoxVerColor_Set(dataGameVersion.Brush);
            ListViewGameVersions.ItemsSource = ItemsGameVersion;
            ListViewGameVersions_Fill();
        }
        //
        public void TBoxVerColor_Set(Brush inputBrush)
        {
            SolidColorBrush brushFixed = inputBrush as SolidColorBrush;
            TBoxVerColor.Text = brushFixed.Color.R.ToString() + ", " + brushFixed.Color.G.ToString() + ", " + brushFixed.Color.B.ToString();
        }
        public void ListViewGameVersions_Fill()
        {
            ItemsGameVersion.Clear();
            List<DataGameVersion> gameVersions = Data_GameVersions_Find_GameVersions(TBoxVerSearch.Text);
            List<DataGameVersion> gameVersionsSorted = Data_GameVersions_Sort(gameVersions);
            for (int key = 0; key < gameVersionsSorted.Count; key++)
            {
                ItemsGameVersion.Add(new ItemGameVersion {IdFake = gameVersionsSorted[key].IdFake, Name = gameVersionsSorted[key].Name, Brush = gameVersionsSorted[key].Brush});
            }
            int idReal = Data_GameVersions_Get_IdReal(gameVersionsSorted, SettingsCurrent.Game.SelectedIdFake);
            ListViewGameVersions.SelectedIndex = idReal;
        }
        // Tab Deep Dive.
        public void Tab_DeepDive_Update()
        {
            List<DataDeepDive> deepDives = Data.DeepDives;
            DataDeepDive dataDeepDive = Data_DeepDives_Get_DeepDive(SettingsCurrent.DeepDive.SelectedIdReal);
            //
            BoxDeepDideInfo_Fill();
            if (SettingsCurrent.DeepDive.SelectedIdReal >= 0)
            {
                TBoxDDSeed.IsEnabled = false;
                TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                BoxDeepDideInfo_Enable(false);
                BoxDeepDideInfo_Set(dataDeepDive);
            }
            else
            {
                TBoxDDSeed.IsEnabled = true;
                TBoxDDSeed.Text = SettingsCurrent.DeepDive.Seed.ToString();
                BoxDeepDideInfo_Enable(true);
            }
            CBoxLostDeepDives.IsChecked = SettingsCurrent.DeepDive.LostDeepDives;
            //
            ListViewDeepDives.ItemsSource = ItemsDeepDive;
            ListViewDeepDives_Fill();
        }
        //
        public void BoxDeepDideInfo_Fill()
        {
            // Normal.
            CBoxDDNorReg.ItemsSource = VariableEnums.Regions;
            CBoxDDNorReg.SelectedIndex = 0;
            CBoxDDNorMisT1.ItemsSource = VariableEnums.Missions;
            CBoxDDNorMisT1.SelectedIndex = 0;
            CBoxDDNorMisT2.ItemsSource = VariableEnums.Missions;
            CBoxDDNorMisT2.SelectedIndex = 0;
            CBoxDDNorMisT3.ItemsSource = VariableEnums.Missions;
            CBoxDDNorMisT3.SelectedIndex = 0;
            CBoxDDNorObjT1.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDNorObjT1.SelectedIndex = 0;
            CBoxDDNorObjT2.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDNorObjT2.SelectedIndex = 0;
            CBoxDDNorObjT3.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDNorObjT3.SelectedIndex = 0;
            CBoxDDNorWar1.ItemsSource = VariableEnums.Warnings;
            CBoxDDNorWar1.SelectedIndex = 0;
            CBoxDDNorWar2.ItemsSource = VariableEnums.Warnings;
            CBoxDDNorWar2.SelectedIndex = 0;
            CBoxDDNorWar3.ItemsSource = VariableEnums.Warnings;
            CBoxDDNorWar3.SelectedIndex = 0;
            CBoxDDNorAno1.ItemsSource = VariableEnums.Anomalies;
            CBoxDDNorAno1.SelectedIndex = 0;
            CBoxDDNorAno2.ItemsSource = VariableEnums.Anomalies;
            CBoxDDNorAno2.SelectedIndex = 0;
            CBoxDDNorAno3.ItemsSource = VariableEnums.Anomalies;
            CBoxDDNorAno3.SelectedIndex = 0;
            // Elite.
            CBoxDDEliReg.ItemsSource = VariableEnums.Regions;
            CBoxDDEliReg.SelectedIndex = 0;
            CBoxDDEliMisT1.ItemsSource = VariableEnums.Missions;
            CBoxDDEliMisT1.SelectedIndex = 0;
            CBoxDDEliMisT2.ItemsSource = VariableEnums.Missions;
            CBoxDDEliMisT2.SelectedIndex = 0;
            CBoxDDEliMisT3.ItemsSource = VariableEnums.Missions;
            CBoxDDEliMisT3.SelectedIndex = 0;
            CBoxDDEliObjT1.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDEliObjT1.SelectedIndex = 0;
            CBoxDDEliObjT2.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDEliObjT2.SelectedIndex = 0;
            CBoxDDEliObjT3.ItemsSource = VariableEnums.SideObjectives;
            CBoxDDEliObjT3.SelectedIndex = 0;
            CBoxDDEliWar1.ItemsSource = VariableEnums.Warnings;
            CBoxDDEliWar1.SelectedIndex = 0;
            CBoxDDEliWar2.ItemsSource = VariableEnums.Warnings;
            CBoxDDEliWar2.SelectedIndex = 0;
            CBoxDDEliWar3.ItemsSource = VariableEnums.Warnings;
            CBoxDDEliWar3.SelectedIndex = 0;
            CBoxDDEliAno1.ItemsSource = VariableEnums.Anomalies;
            CBoxDDEliAno1.SelectedIndex = 0;
            CBoxDDEliAno2.ItemsSource = VariableEnums.Anomalies;
            CBoxDDEliAno2.SelectedIndex = 0;
            CBoxDDEliAno3.ItemsSource = VariableEnums.Anomalies;
            CBoxDDEliAno3.SelectedIndex = 0;
        }
        public void BoxDeepDideInfo_Enable(bool inputEnable)
        {
            // Normal.
            TBoxDDNorName.IsEnabled = inputEnable;
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
            TBoxDDEliName.IsEnabled = inputEnable;
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
        public void BoxDeepDideInfo_Set(DataDeepDive inputDeepDive)
        {
            // Normal.
            TBoxDDNorTag.Text = inputDeepDive.Normal.Tag;
            TBoxDDNorName.Text = inputDeepDive.Normal.Name;
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
            TBoxDDEliTag.Text = inputDeepDive.Elite.Tag;
            TBoxDDEliName.Text = inputDeepDive.Elite.Name;
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
        public void ListViewDeepDives_Fill()
        {
            ItemsDeepDive.Clear();
            List<DataDeepDive> dataDeepDives = Data_DeepDives_Find_DeepDives(TBoxDeepDiveSearch.Text);
            List<DataDeepDive> dataDeepDivesSorted = Data_DeepDives_Sort(dataDeepDives);
            for (int key = 0; key < dataDeepDivesSorted.Count; key++)
            {
                string nameNormal;
                if (dataDeepDivesSorted[key].Normal.Tag == "")
                {
                    nameNormal = dataDeepDivesSorted[key].Normal.Name;
                }
                else
                {
                    nameNormal = dataDeepDivesSorted[key].Normal.Tag;
                }
                string nameElite;
                if (dataDeepDivesSorted[key].Elite.Tag == "")
                {
                    nameElite = dataDeepDivesSorted[key].Elite.Name;
                }
                else
                {
                    nameElite = dataDeepDivesSorted[key].Elite.Tag;
                }
                ItemsDeepDive.Add(new ItemDeepDive {IdFake = dataDeepDivesSorted[key].IdFake, Date = dataDeepDivesSorted[key].Date, NameNormal = nameNormal, NameElite = nameElite, Brush = dataDeepDivesSorted[key].Brush});
            }
            int idReal = Data_DeepDives_Get_IdReal(dataDeepDivesSorted, SettingsCurrent.DeepDive.SelectedIdFake);
            ListViewDeepDives.SelectedIndex = idReal;
        }
        // Tab Event.
        public void Tab_Event_Update()
        {
            List<DataEvent> events = Data.Events;
            DataEvent dataEvent = Data_Events_Get_Event(SettingsCurrent.Event.SelectedIdReal);
            //
            if (SettingsCurrent.Event.SelectedIdReal >= 0)
            {
                TBoxEventCommand.IsEnabled = false;
                TBoxEventCommand.Text = dataEvent.Command;
            }
            else
            {
                TBoxEventCommand.IsEnabled = true;
                TBoxEventCommand.Text = SettingsCurrent.Event.Command;
            }
            CBoxFreeBeers.IsChecked = SettingsCurrent.Event.FreeBeers;
            CBoxLostEvents.IsChecked = SettingsCurrent.Event.LostEvents;
            //
            ListViewEvent.ItemsSource = ItemsEvent;
            ListViewEvents_Fill();
            //
            ListViewEventItems.ItemsSource = ItemsEventItem;
            ListViewEventItems_Fill();
        }
        //
        public void ListViewEvents_Fill()
        {
            ItemsEvent.Clear();
            List<DataEvent> dataEvents = Data_Events_Find_Events(TextBoxEventSearch.Text);
            List<DataEvent> dataEventsSorted = Data_Events_Sort(dataEvents);
            for (int key = 0; key < dataEventsSorted.Count; key++)
            {
                ItemsEvent.Add(new ItemEvent { IdFake = dataEventsSorted[key].IdFake, Year = dataEventsSorted[key].Year, Name = dataEventsSorted[key].Name });
            }
            int idReal = Data_Events_Get_IdReal(dataEventsSorted, SettingsCurrent.Event.SelectedIdFake);
            ListViewEvent.SelectedIndex = idReal;
        }
        public void ListViewEventItems_Fill()
        {
            ItemsEventItem.Clear();
            if (SettingsCurrent.Event.SelectedIdReal >= 0)
            {
                for (int key = 0; key < Data.Events[SettingsCurrent.Event.SelectedIdReal].Items.Count; key++)
                {
                    ItemsEventItem.Add(new ItemEventItem { Type = Data.Events[SettingsCurrent.Event.SelectedIdReal].Items[key].Type, Name = Data.Events[SettingsCurrent.Event.SelectedIdReal].Items[key].Name });
                }
            }
        }
        // Tab Assignment.
        public void Tab_Assignment_Update()
        {
            uint seed = SettingsCurrent.Assignment.Seed;
            //
            TBoxAssSeed.Text = seed.ToString();
        }
        // Tab Settings.
        public void Tab_Settings_Update()
        {
            double positionX = SettingsCurrent.Settings.PositionX;
            double positionY = SettingsCurrent.Settings.PositionY;
            double sizeX = SettingsCurrent.Settings.SizeX;
            double sizeY = SettingsCurrent.Settings.SizeY;
            bool hideNotes = SettingsCurrent.Settings.HideNotes;
            //
            TBoxPosX.Text = positionX.ToString();
            TBoxPosY.Text = positionY.ToString();
            TBoxSizeX.Text = sizeX.ToString();
            TBoxSizeY.Text = sizeY.ToString();
            CBoxHideNotes.IsChecked = hideNotes;
            //
            Tab_Settings_Position(positionX, positionY);
            Tab_Settings_Size(sizeX, sizeY);
            Tab_Settings_HideNotes(hideNotes);
        }
        public void Tab_Settings_Position(double inputPositionX, double inputPositionY)
        {
            this.Left = inputPositionX;
            this.Top = inputPositionY;
        }
        public void Tab_Settings_Size(double inputSizeX, double inputSizeY)
        {
            this.Width = inputSizeX;
            this.Height = inputSizeY;
        }
        public void Tab_Settings_HideNotes(bool inputHideNotes)
        {
            if (inputHideNotes == true)
            {
                GridServices.RowDefinitions[2].Height = new GridLength(0);
                GridGame.RowDefinitions[2].Height = new GridLength(0);
                GridDeepDive.RowDefinitions[2].Height = new GridLength(0);
                GridEvent.RowDefinitions[2].Height = new GridLength(0);
                GridAssignment.RowDefinitions[1].Height = new GridLength(0);
            }
            else
            {
                GridServices.RowDefinitions[2].Height = new GridLength(0.15, GridUnitType.Star);
                GridGame.RowDefinitions[2].Height = new GridLength(0.15, GridUnitType.Star);
                GridDeepDive.RowDefinitions[2].Height = new GridLength(0.15, GridUnitType.Star);
                GridEvent.RowDefinitions[2].Height = new GridLength(0.15, GridUnitType.Star);
                GridAssignment.RowDefinitions[1].Height = new GridLength(0.15, GridUnitType.Star);
            }
        }
        // Timers.
        public void Timer_SearchGameVersion_Start()
        {
            TimerSearchGameVersion.Start();
        }
        public void Timer_SearchGameVersion_Stop()
        {
            TimerSearchGameVersion.Stop();
        }
        public void Timer_SearchGameVersion_Update()
        {
            Timer_SearchGameVersion_Stop();
            Timer_SearchGameVersion_Start();
        }
        public void Timer_SearchDeepDive_Start()
        {
            TimerSearchDeepDive.Start();
        }
        public void Timer_SearchDeepDive_Stop()
        {
            TimerSearchDeepDive.Stop();
        }
        public void Timer_SearchDeepDive_Update()
        {
            Timer_SearchDeepDive_Stop();
            Timer_SearchDeepDive_Start();
        }
        public void Timer_SearchEvent_Start()
        {
            TimerSearchEvent.Start();
        }
        public void Timer_SearchEvent_Stop()
        {
            TimerSearchEvent.Stop();
        }
        public void Timer_SearchEvent_Update()
        {
            Timer_SearchEvent_Stop();
            Timer_SearchEvent_Start();
        }
        public void Timer_WindowPosition_Start()
        {
            TimerWindowPosition.Start();
        }
        public void Timer_WindowPosition_Stop()
        {
            TimerWindowPosition.Stop();
        }
        public void Timer_WindowPosition_Update()
        {
            Timer_WindowPosition_Stop();
            Timer_WindowPosition_Start();
        }
        public void Timer_WindowSize_Start()
        {
            TimerWindowSize.Start();
        }
        public void Timer_WindowSize_Stop()
        {
            TimerWindowSize.Stop();
        }
        public void Timer_WindowSize_Update()
        {
            Timer_WindowSize_Stop();
            Timer_WindowSize_Start();
        }



        #region Triggers
        // Window.
        private void Window_Initialized(object sender, EventArgs e)
        {
            // In case of shutting down the app.exe with Task Manager and leaving server processes running.
            Server_Stop();
            // Load data required for app functionality.
            Data_All_Load();
            // Load settings.
            Settings_All_Load();
            // Tabs.
            Tab_All_Update();





            string path = Path.Combine(PathFolderApplication, @"Data\Languages\English\Help.txt");
            TextRange range;
            FileStream fStream;
            if (File.Exists(path))
            {
                range = new TextRange(Help.Document.ContentStart, Help.Document.ContentEnd);
                fStream = new FileStream(path, FileMode.OpenOrCreate);
                range.Load(fStream, DataFormats.Text);
                fStream.Close();
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Server_Stop();
            Tab_Services_ChangeRedirects_Remove(SettingsCurrent.Services.ChangeRedirects);
        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            // Check if window is not minimized.
            // When window is minimized, it's position will be -32000, -32000.
            if (this.WindowState != WindowState.Minimized)
            {
                Timer_WindowPosition_Update();
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Check if window is not minimized.
            // When window is minimized, it's size will be 0, 0.
            if (this.WindowState != WindowState.Minimized)
            {
                Timer_WindowSize_Update();
            }
        }
        // Tab Services.
        private void TBoxIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string ip = TBoxIP.Text;
            // Check to avoid double trigger.
            if (ip != SettingsCurrent.Services.IP)
            {
                Settings_Services_IP_Save(ip);
            }
        }
        private void ButtonRedsAdd_Click(object sender, RoutedEventArgs e)
        {
            // Remove redirects to avoid duplication and remove old version.
            Redirects_Remove();
            // Add redirects.
            Redirects_Add();
            // Update info panel.
            bool redirectsAdded = Redirects_CheckAdded();
            CBoxRedsAdded_Set(redirectsAdded);
            // Update timer, in case changes were not picked up instantly.
            if (redirectsAdded == false)
            {
                TimerRedsAdded_Update();
            }
        }
        private void ButtonRedsRemove_Click(object sender, RoutedEventArgs e)
        {
            // Remove redirects.
            Redirects_Remove();
            // Update info panel.
            bool redirectsAdded = Redirects_CheckAdded();
            CBoxRedsAdded_Set(redirectsAdded);
            // Update timer, in case changes were not picked up instantly.
            if (redirectsAdded == false)
            {
                TimerRedsAdded_Update();
            }
        }
        private void CBoxRedsChange_Click(object sender, RoutedEventArgs e)
        {
            bool changeRedirects = CBoxRedsChange.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (changeRedirects != SettingsCurrent.Services.ChangeRedirects)
            {
                Settings_Services_ChangeRedirects_Save(changeRedirects);
            }
        }
        private void ButtonCertsAdd_Click(object sender, RoutedEventArgs e)
        {
            // Remove old version, no need to remove for dublication, because certificates can't be duplicated.
            Certificates_Remove();
            // Add certificates.
            Certificates_Add();
            // Update info panel.
            bool certificatesAdded = Certificates_CheckAdded();
            CBoxCertsAdded_Set(certificatesAdded);
            // Update timer, in case changes were not picked up instantly.
            if (certificatesAdded == false)
            {
                TimerCertsAdded_Update();
            }
        }
        private void ButtonCertsRemove_Click(object sender, RoutedEventArgs e)
        {
            // Remove certificates.
            Certificates_Remove();
            // Update info panel.
            bool certificatesAdded = Certificates_CheckAdded();
            CBoxCertsAdded_Set(certificatesAdded);
            // Update timer, in case changes were not picked up instantly.
            if (certificatesAdded == false)
            {
                TimerCertsAdded_Update();
            }
        }
        private void ButtonRedsOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            Redirects_OpenFolder();
        }
        private void ButtonCertsOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Certificates_OpenFolder();
        }
        private void ButtonServStart_Click(object sender, RoutedEventArgs e)
        {
            if (Server_CheckRunning() == true)
            {
                string title = this.Title;
                string message = "Server is already started. Do you want to stop current server and start new one?";
                MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Server_Stop();
                    Server_Start();
                    // Update info panel.
                    bool serverRunning = Server_CheckRunning();
                    CBoxServRunning_Set(serverRunning);
                    // Update timer, in case changes were not picked up instantly.
                    if (serverRunning == false)
                    {
                        TimerServRunning_Update();
                    }
                }
            }
            else
            {
                Server_Start();
                // Update info panel.
                bool serverRunning = Server_CheckRunning();
                CBoxServRunning_Set(serverRunning);
                // Update timer, in case changes were not picked up instantly.
                if (serverRunning == false)
                {
                    TimerServRunning_Update();
                }
            }
        }
        private void ButtonServStop_Click(object sender, RoutedEventArgs e)
        {
            Server_Stop();
            // Update info panel.
            bool serverRunning = Server_CheckRunning();
            CBoxServRunning_Set(serverRunning);
            // Update timer, in case changes were not picked up instantly.
            if (serverRunning == false)
            {
                TimerServRunning_Update();
            }
        }
        private void CBoxServStart_Click(object sender, RoutedEventArgs e)
        {
            bool startOnLaunch = CBoxServStart.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (startOnLaunch != SettingsCurrent.Services.StartServer)
            {
                Settings_Services_StartServer_Save(startOnLaunch);
            }
        }
        private void TimerServicesStatus_Tick(object sender, EventArgs e)
        {
            bool redirectsAdded = Redirects_CheckAdded();
            CBoxRedsAdded_Set(redirectsAdded);
            bool certificatesAdded = Certificates_CheckAdded();
            CBoxCertsAdded_Set(certificatesAdded);
            bool serverRunning = Server_CheckRunning();
            CBoxServRunning_Set(serverRunning);
        }
        private void TimerServicesStatusRedirects_Tick(object sender, EventArgs e)
        {
            bool redirectsAdded = Redirects_CheckAdded();
            CBoxRedsAdded_Set(redirectsAdded);
            TimerRedsAdded_Stop();
        }
        private void TimerServicesStatusCertificates_Tick(object sender, EventArgs e)
        {
            bool certificatesAdded = Certificates_CheckAdded();
            CBoxCertsAdded_Set(certificatesAdded);
            TimerCertsAdded_Stop();
        }
        private void TimerServicesStatusServer_Tick(object sender, EventArgs e)
        {
            bool serverRunning = Server_CheckRunning();
            CBoxServRunning_Set(serverRunning);
            TimerServRunning_Stop();
        }
        // Tab Game.
        private void TBoxPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            string path = TBoxPath.Text;
            int idReal = SettingsCurrent.Game.SelectedIdReal;
            if (idReal >= 0)
            {
                // Check to avoid double trigger.
                if (path != Data.GameVersions[idReal].Path)
                {
                    Data_GameVersions_Save_Path(idReal, path);
                }
            }
            else
            {
                // Check to avoid double trigger.
                if (path != SettingsCurrent.Game.Path)
                {
                    Settings_Game_Path_Save(path);
                }
            }
        }
        private void ButtonPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                string path = dialog.FileName;
                int idReal = SettingsCurrent.Game.SelectedIdReal;
                if (idReal >= 0)
                {
                    // Check to avoid double trigger.
                    if (path != Data.GameVersions[idReal].Path)
                    {
                        Data_GameVersions_Save_Path(idReal, path);
                        //
                        TBoxPath.Text = path;
                    }
                }
                else
                {
                    // Check to avoid double trigger.
                    if (path != SettingsCurrent.Game.Path)
                    {
                        Settings_Game_Path_Save(path);
                        //
                        TBoxPath.Text = path;
                    }
                }
            }
        }
        private void ButtonGameLaunch_Click(object sender, RoutedEventArgs e)
        {
            string path;
            if (SettingsCurrent.Game.SelectedIdReal >= 0)
            {
                path = Data.GameVersions[SettingsCurrent.Game.SelectedIdReal].Path;
            }
            else
            {
                path = SettingsCurrent.Game.Path;
            }
            //
            if (path != "")
            {
                bool? check = Game_Path_Check(path);
                if (check != null)
                {
                    if (check == true)
                    {
                        Game_Start(path);
                    }
                    else
                    {
                        // File is not FSD-Win64-Shipping.exe.
                        string title = this.Title;
                        string message = "File \"" + path + "\" is not correct executable file.";
                        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // File doesn't exist.
                    string title = this.Title;
                    string message = "File \"" + path + "\" doesn't exist.";
                    System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // User didn't define path.
                string title = this.Title;
                string message = "Executable Path can't be empty.";
                System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TBoxPlrId_TextChanged(object sender, TextChangedEventArgs e)
        {
            string playerId = TBoxPlrId.Text;
            // Check to avoid double trigger.
            if (playerId != SettingsCurrent.Game.PlayerId)
            {
                Settings_Game_PlayerId_Save(playerId);
            }
        }
        private void TBoxPlrName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string playerName = TBoxPlrName.Text;
            // Check to avoid double trigger.
            if (playerName != "" && playerName != SettingsCurrent.Game.PlayerName)
            {
                Settings_Game_PlayerName_Save(playerName);
            }
        }
        private void TBoxPlrName_LostFocus(object sender, RoutedEventArgs e)
        {
            string setting = TBoxPlrName.Text;
            //
            if (setting == "")
            {
                Settings_Game_PlayerName_Save(setting);
                //
                TBoxPlrName.Text = SettingsDefault.Game.PlayerName;
            }
        }
        private void TBoxLnchParams_TextChanged(object sender, TextChangedEventArgs e)
        {
            string command = TBoxLnchParams.Text;
            // Check to avoid double trigger.
            if (command != SettingsCurrent.Game.Command)
            {
                Settings_Game_Command_Save(command);
            }
        }
        private void ButtonVerNumber_Click(object sender, RoutedEventArgs e)
        {
            string versionNumber = TBoxVerNumber.Text;
            if (versionNumber != "")
            {
                Clipboard.SetText(versionNumber);
            }
        }
        private void TBoxVerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = TBoxVerName.Text;
            int idReal = SettingsCurrent.Game.SelectedIdReal;
            if (idReal >= 0)
            {
                // Check to avoid double trigger.
                if (name != Data.GameVersions[idReal].Name)
                {
                    Data_GameVersions_Save_Name(idReal, name);
                    //
                    ListViewGameVersions_Fill();
                }
            }
        }
        private void ButtonVerName_Click(object sender, RoutedEventArgs e)
        {
            string versionName = TBoxVerName.Text;
            if (versionName != "")
            {
                Clipboard.SetText(versionName);
            }
        }
        private void TBoxVerManifest_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_Manifest(TBoxVerManifest.Text, TBoxVerManifest.SelectionStart, out string str, out int pos);
            TBoxVerManifest.Text = str;
            TBoxVerManifest.SelectionStart = pos;
        }
        private void ButtonVerManifest_Click(object sender, RoutedEventArgs e)
        {
            string manifest = TBoxVerManifest.Text;
            if (manifest != "")
            {
                Clipboard.SetText(manifest);
            }
        }
        private void ButtonVerColor_Click(object sender, RoutedEventArgs e)
        {
            string manifest = TBoxVerColor.Text;
            if (manifest != "")
            {
                Clipboard.SetText(manifest);
            }
        }
        private void TBoxVerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TBoxVerSearch.Text;
            if (search != SettingsCurrent.Game.Search)
            {
                Timer_SearchGameVersion_Update();
            }
        }
        private void ListViewGameVersions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When item is selected, selected amount will be set to 0 first, then to 1, executing the trigger.
            if (ListViewGameVersions.SelectedIndex >= 0)
            {
                string idFake = ItemsGameVersion[ListViewGameVersions.SelectedIndex].IdFake;
                // Check to avoid double trigger.
                if (idFake != SettingsCurrent.Game.SelectedIdFake)
                {
                    if (SettingsCurrent.DeepDive.SelectedIdReal >= 0 && idFake != Data.DeepDives[SettingsCurrent.DeepDive.SelectedIdReal].GameVersion)
                    {
                        string title = this.Title;
                        string message = "This will deselect current Deep Dive. Do you still want to continue?";
                        MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            int idReal = Data_GameVersions_Get_IdReal(Data.GameVersions, idFake);
                            DataGameVersion dataGameVersion = Data_GameVersions_Get_GameVersion(idReal);
                            //
                            Settings_Game_SelectedId_Save(idReal, dataGameVersion.IdFake);
                            //
                            TBoxPath.Text = dataGameVersion.Path;
                            TBoxVerNumber.IsEnabled = false;
                            TBoxVerNumber.Text = dataGameVersion.IdFake;
                            TBoxVerName.Text = dataGameVersion.Name;
                            TBoxVerManifest.IsEnabled = false;
                            TBoxVerManifest.Text = dataGameVersion.Manifest;
                            TBoxVerColor_Set(dataGameVersion.Brush);
                            //
                            DataDeepDive dataDeepDive = new DataDeepDive();
                            //
                            Settings_DeepDive_SelectedId_Save(-1, "");
                            Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                            //
                            TBoxDDSeed.IsEnabled = true;
                            TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                            BoxDeepDideInfo_Enable(true);
                            BoxDeepDideInfo_Set(dataDeepDive);
                            ListViewDeepDives.SelectedIndex = -1;
                        }
                        else {
                            // Fix for right clicking on the item and pressing "No".
                            // Item will switch outline if not fixed.
                            ListViewGameVersions_Fill();
                        }
                    }
                    else
                    {
                        int idReal = Data_GameVersions_Get_IdReal(Data.GameVersions, idFake);
                        DataGameVersion dataGameVersion = Data_GameVersions_Get_GameVersion(idReal);
                        //
                        Settings_Game_SelectedId_Save(idReal, dataGameVersion.IdFake);
                        //
                        TBoxPath.Text = dataGameVersion.Path;
                        TBoxVerNumber.IsEnabled = false;
                        TBoxVerNumber.Text = dataGameVersion.IdFake;
                        TBoxVerName.Text = dataGameVersion.Name;
                        TBoxVerManifest.IsEnabled = false;
                        TBoxVerManifest.Text = dataGameVersion.Manifest;
                        TBoxVerColor_Set(dataGameVersion.Brush);
                    }
                }
            }
        }
        private void ButtonVerDeselect_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsCurrent.Game.SelectedIdReal >= 0)
            {
                if (SettingsCurrent.DeepDive.SelectedIdReal >= 0 && SettingsCurrent.Game.SelectedIdFake == Data.DeepDives[SettingsCurrent.DeepDive.SelectedIdReal].GameVersion)
                {
                    string title = this.Title;
                    string message = "This will deselect current Deep Dive. Do you still want to continue?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataGameVersion dataGameVersion = new DataGameVersion();
                        //
                        Settings_Game_SelectedId_Save(-1, "");
                        //
                        TBoxPath.Text = SettingsCurrent.Game.Path;
                        TBoxVerNumber.IsEnabled = true;
                        TBoxVerNumber.Text = dataGameVersion.IdFake;
                        TBoxVerName.Text = dataGameVersion.Name;
                        TBoxVerManifest.IsEnabled = true;
                        TBoxVerManifest.Text = dataGameVersion.Manifest;
                        TBoxVerColor_Set(dataGameVersion.Brush);
                        ListViewGameVersions.SelectedIndex = -1;

                        //
                        DataDeepDive dataDeepDive = new DataDeepDive();
                        //
                        Settings_DeepDive_SelectedId_Save(-1, "");
                        Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                        //
                        TBoxDDSeed.IsEnabled = true;
                        TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                        BoxDeepDideInfo_Enable(true);
                        BoxDeepDideInfo_Set(dataDeepDive);
                        ListViewDeepDives.SelectedIndex = -1;
                    }
                }
                else
                {
                    DataGameVersion dataGameVersion = new DataGameVersion();
                    //
                    Settings_Game_SelectedId_Save(-1, "");
                    //
                    TBoxPath.Text = SettingsCurrent.Game.Path;
                    TBoxVerNumber.IsEnabled = true;
                    TBoxVerNumber.Text = dataGameVersion.IdFake;
                    TBoxVerName.Text = dataGameVersion.Name;
                    TBoxVerManifest.IsEnabled = true;
                    TBoxVerManifest.Text = dataGameVersion.Manifest;
                    TBoxVerColor_Set(dataGameVersion.Brush);
                    ListViewGameVersions.SelectedIndex = -1;
                }
            }
        }
        private void TimerSearchGameVersion_Tick(object sender, EventArgs e)
        {
            string search = TBoxVerSearch.Text;
            if (search != SettingsCurrent.Game.Search)
            {
                Settings_Game_Search_Save(search);
                //
                ListViewGameVersions_Fill();
                Timer_SearchGameVersion_Stop();
            }
        }
        // Tab Deep Dive.
        private void TBoxDeepDiveSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxDDSeed.Text, TBoxDDSeed.SelectionStart, out string str, out int pos);
            TBoxDDSeed.Text = str;
            TBoxDDSeed.SelectionStart = pos;
            // Try to parse, if can't parse don't save, because string can only be empty, and emty string will be reset to default on focus loss.
            if (uint.TryParse(TBoxDDSeed.Text, out uint seed) == true && seed != SettingsCurrent.DeepDive.Seed)
            {
                Settings_DeepDive_Seed_Save(seed);
            }
        }
        private void TBoxDeepDiveSeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxDDSeed.Text == "")
            {
                Settings_DeepDive_Seed_Save(SettingsCurrent.DeepDive.Seed);
                //
                TBoxDDSeed.Text = SettingsCurrent.DeepDive.Seed.ToString();
            }
        }
        private void ButtonDeepDiveSeed_Click(object sender, RoutedEventArgs e)
        {
            uint seed = Seed_Generate();
            if (SettingsCurrent.DeepDive.SelectedIdReal >= 0)
            {
                string title = this.Title;
                string message = "This will deselect current deep dive. Do you want to continue?";
                MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DataDeepDive dataDeepDive = new DataDeepDive();
                    //
                    Settings_DeepDive_Seed_Save(seed);
                    Settings_DeepDive_SelectedId_Save(-1, "");
                    //
                    TBoxDDSeed.IsEnabled = true;
                    TBoxDDSeed.Text = seed.ToString();
                    BoxDeepDideInfo_Enable(true);
                    BoxDeepDideInfo_Set(dataDeepDive);
                    ListViewDeepDives.SelectedIndex = -1;
                }
            }
            else
            {
                // Check to avoid double trigger.
                if (seed != SettingsCurrent.DeepDive.Seed)
                {
                    Settings_DeepDive_Seed_Save(seed);
                    //
                    TBoxDDSeed.Text = seed.ToString();
                }
            }
        }
        private void CBoxLostDeepDives_Click(object sender, RoutedEventArgs e)
        {
            bool setting = CBoxLostDeepDives.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (setting != SettingsCurrent.DeepDive.LostDeepDives)
            {
                Settings_DeepDive_LostDeepDives_Save(setting);
                ListViewDeepDives_Fill();
            }
        }
        private void TBoxDDNorTag_TextChanged(object sender, global::System.EventArgs e)
        {
            string tagNormal = TBoxDDNorTag.Text;
            int idReal = SettingsCurrent.DeepDive.SelectedIdReal;
            if (idReal >= 0)
            {
                // Check to avoid double trigger.
                if (tagNormal != Data.DeepDives[idReal].Normal.Tag)
                {
                    Data_DeepDives_Save_TagNormal(idReal, tagNormal);
                    //
                    ListViewDeepDives_Fill();
                }
            }
        }
        private void TBoxDDEliTag_TextChanged(object sender, global::System.EventArgs e)
        {
            string tagElite = TBoxDDEliTag.Text;
            int idReal = SettingsCurrent.DeepDive.SelectedIdReal;
            if (idReal >= 0)
            {
                // Check to avoid double trigger.
                if (tagElite != Data.DeepDives[idReal].Elite.Tag)
                {
                    Data_DeepDives_Save_TagElite(idReal, tagElite);
                    //
                    ListViewDeepDives_Fill();
                }
            }
        }
        private void TBoxDeepDiveSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TBoxDeepDiveSearch.Text;
            if (search != SettingsCurrent.DeepDive.Search)
            {
                Timer_SearchDeepDive_Update();
            }
        }
        private void ListViewDeepDives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When item is selected, selected amount will be set to 0 first, then to 1, executing the trigger.
            if (ListViewDeepDives.SelectedIndex >= 0)
            {
                string idFake = ItemsDeepDive[ListViewDeepDives.SelectedIndex].IdFake;
                // Check to avoid double trigger.
                if (idFake != SettingsCurrent.DeepDive.SelectedIdFake)
                {
                    int idReal = Data_DeepDives_Get_IdReal(Data.DeepDives, idFake);
                    DataDeepDive dataDeepDive = Data_DeepDives_Get_DeepDive(idReal);
                    //
                    Settings_DeepDive_SelectedId_Save(idReal, dataDeepDive.IdFake);
                    Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                    //
                    TBoxDDSeed.IsEnabled = false;
                    TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                    BoxDeepDideInfo_Enable(false);
                    BoxDeepDideInfo_Set(dataDeepDive);
                    //
                    if (SettingsCurrent.Game.SelectedIdFake != Data.DeepDives[idReal].GameVersion)
                    {
                        int idRealGameVersion = Data_GameVersions_Get_IdReal(Data.GameVersions, dataDeepDive.GameVersion);
                        DataGameVersion dataGameVersion = Data_GameVersions_Get_GameVersion(idRealGameVersion);
                        //
                        Settings_Game_SelectedId_Save(idRealGameVersion, dataGameVersion.IdFake);
                        //
                        TBoxPath.Text = dataGameVersion.Path;
                        TBoxVerNumber.IsEnabled = false;
                        TBoxVerNumber.Text = dataGameVersion.IdFake;
                        TBoxVerName.Text = dataGameVersion.Name;
                        TBoxVerManifest.IsEnabled = false;
                        TBoxVerManifest.Text = dataGameVersion.Manifest;
                        TBoxVerColor_Set(dataGameVersion.Brush);
                        ListViewGameVersions_Fill();
                    }
                    if (SettingsCurrent.Event.SelectedIdFake != Data.DeepDives[idReal].Event)
                    {
                        int idRealEvent = Data_Events_Get_IdReal(Data.Events, dataDeepDive.Event);
                        DataEvent dataEvent = Data_Events_Get_Event(idRealEvent);
                        //
                        Settings_Event_SelectedId_Save(idRealEvent, dataEvent.IdFake);
                        Settings_Event_Command_Save(dataEvent.Command);
                        //
                        TBoxEventCommand.IsEnabled = false;
                        TBoxEventCommand.Text = dataEvent.Command;
                        ListViewEvents_Fill();
                        ListViewEventItems_Fill();
                    }
                }
            }
        }
        private void ButtonDeepDiveDeselect_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsCurrent.DeepDive.SelectedIdReal >= 0)
            {
                DataDeepDive dataDeepDive = new DataDeepDive();
                //
                Settings_DeepDive_SelectedId_Save(-1, "");
                Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                //
                TBoxDDSeed.IsEnabled = true;
                TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                BoxDeepDideInfo_Enable(true);
                BoxDeepDideInfo_Set(dataDeepDive);
                ListViewDeepDives.SelectedIndex = -1;
            }
        }
        private void TimerSearchDeepDive_Tick(global::System.Object sender, global::System.EventArgs e)
        {
            string search = TBoxDeepDiveSearch.Text;
            if (search != SettingsCurrent.DeepDive.Search)
            {
                Settings_DeepDive_Search_Save(search);
                //
                ListViewDeepDives_Fill();
                Timer_SearchDeepDive_Stop();
            }
        }
        // Tab Event.
        private void TextBoxEventCommand_TextChanged(object sender, TextChangedEventArgs e)
        {
            string command = TBoxEventCommand.Text;
            // Check to avoid double trigger.
            if (command != SettingsCurrent.Event.Command)
            {
                Settings_Event_Command_Save(command);
            }
        }
        private void ButtonEventCommandClear_Click(object sender, RoutedEventArgs e)
        {
            string command = TBoxEventCommand.Text;
            //
            if (SettingsCurrent.Event.SelectedIdReal >= 0)
            {
                string title = this.Title;
                string message = "This will deselect current event. Do you want to continue?";
                MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DataEvent dataEvent = new DataEvent();
                    //
                    Settings_Event_Command_Save("");
                    Settings_Event_SelectedId_Save(-1, "");
                    //
                    TBoxEventCommand.IsEnabled = true;
                    TBoxEventCommand.Text = "";
                    ListViewEvent.SelectedIndex = -1;
                    ListViewEvents_Fill();
                    ListViewEventItems_Fill();
                }
            }
            else
            {
                // Check to avoid double trigger.
                if (command != "")
                {
                    Settings_Event_Command_Save("");
                    //
                    TBoxEventCommand.Text = "";
                }
            }
        }
        private void CheckBoxFreeBeers_Click(object sender, RoutedEventArgs e)
        {
            bool freeBeers = CBoxFreeBeers.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (freeBeers != SettingsCurrent.Event.FreeBeers)
            {
                Settings_Event_FreeBeers_Save(freeBeers);
            }
        }
        private void CBoxLostEvents_Click(object sender, RoutedEventArgs e)
        {
            bool setting = CBoxLostEvents.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (setting != SettingsCurrent.Event.LostEvents)
            {
                Settings_Event_LostEvents_Save(setting);
                ListViewEvents_Fill();
                ListViewEventItems_Fill();
            }
        }
        private void TextBoxEventSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = TextBoxEventSearch.Text;
            if (search != SettingsCurrent.Event.Search)
            {
                Timer_SearchEvent_Update();
            }
        }
        private void ListViewEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When item is selected, selected amount will be set to 0 first, then to 1, executing the trigger.
            if (ListViewEvent.SelectedIndex >= 0)
            {
                string idFake = ItemsEvent[ListViewEvent.SelectedIndex].IdFake;
                // Check to avoid double trigger.
                if (idFake != SettingsCurrent.Event.SelectedIdFake)
                {
                    if (SettingsCurrent.DeepDive.SelectedIdReal >= 0 && idFake != Data.DeepDives[SettingsCurrent.DeepDive.SelectedIdReal].GameVersion)
                    {
                        string title = this.Title;
                        string message = "This will deselect current Deep Dive. Do you still want to continue?";
                        MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            int idReal = Data_Events_Get_IdReal(Data.Events, idFake);
                            DataEvent dataEvent = Data_Events_Get_Event(idReal);
                            //
                            Settings_Event_SelectedId_Save(idReal, dataEvent.IdFake);
                            Settings_Event_Command_Save(dataEvent.Command);
                            //
                            TBoxEventCommand.IsEnabled = false;
                            TBoxEventCommand.Text = dataEvent.Command;
                            ListViewEventItems_Fill();
                            //
                            DataDeepDive dataDeepDive = new DataDeepDive();
                            //
                            Settings_DeepDive_SelectedId_Save(-1, "");
                            Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                            //
                            TBoxDDSeed.IsEnabled = true;
                            TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                            BoxDeepDideInfo_Enable(true);
                            BoxDeepDideInfo_Set(dataDeepDive);
                            ListViewDeepDives.SelectedIndex = -1;
                        }
                        else
                        {
                            // Fix for right clicking on the item and pressing "No".
                            // Item will switch outline if not fixed.
                            ListViewEvents_Fill();
                        }
                    }
                    else
                    {
                        int idReal = Data_Events_Get_IdReal(Data.Events, idFake);
                        DataEvent dataEvent = Data_Events_Get_Event(idReal);
                        //
                        Settings_Event_SelectedId_Save(idReal, dataEvent.IdFake);
                        Settings_Event_Command_Save(dataEvent.Command);
                        //
                        TBoxEventCommand.IsEnabled = false;
                        TBoxEventCommand.Text = dataEvent.Command;
                        ListViewEventItems_Fill();
                    }
                }
            }
        }
        private void ButtonEventDeselect_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsCurrent.Event.SelectedIdReal >= 0)
            {
                if (SettingsCurrent.DeepDive.SelectedIdReal >= 0 && SettingsCurrent.Event.SelectedIdFake == Data.DeepDives[SettingsCurrent.DeepDive.SelectedIdReal].Event)
                {
                    string title = this.Title;
                    string message = "This will deselect current Deep Dive. Do you still want to continue?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataEvent dataEvent = new DataEvent();
                        //
                        Settings_Event_SelectedId_Save(-1, "");
                        Settings_Event_Command_Save(dataEvent.Command);
                        //
                        TBoxEventCommand.IsEnabled = true;
                        TBoxEventCommand.Text = dataEvent.Command;
                        ListViewEvent.SelectedIndex = -1;
                        ListViewEventItems_Fill();
                        //
                        DataDeepDive dataDeepDive = new DataDeepDive();
                        //
                        Settings_DeepDive_SelectedId_Save(-1, "");
                        Settings_DeepDive_Seed_Save(dataDeepDive.Seed);
                        //
                        TBoxDDSeed.IsEnabled = true;
                        TBoxDDSeed.Text = dataDeepDive.Seed.ToString();
                        BoxDeepDideInfo_Enable(true);
                        BoxDeepDideInfo_Set(dataDeepDive);
                        ListViewDeepDives.SelectedIndex = -1;
                    }
                }
                else
                {
                    DataEvent dataEvent = new DataEvent();
                    //
                    Settings_Event_SelectedId_Save(-1, "");
                    Settings_Event_Command_Save(dataEvent.Command);
                    //
                    TBoxEventCommand.IsEnabled = true;
                    TBoxEventCommand.Text = dataEvent.Command;
                    ListViewEvent.SelectedIndex = -1;
                    ListViewEventItems_Fill();
                }
            }
        }
        private void TimerSearchEvent_Tick(global::System.Object sender, global::System.EventArgs e)
        {
            string search = TextBoxEventSearch.Text;
            if (search != SettingsCurrent.Event.Search)
            {
                Settings_Event_Search_Save(search);
                //
                ListViewEvents_Fill();
                Timer_SearchEvent_Stop();
            }
        }
        // Tab Assignment.
        private void TBoxAssSeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxAssSeed.Text, TBoxAssSeed.SelectionStart, out string str, out int pos);
            TBoxAssSeed.Text = str;
            TBoxAssSeed.SelectionStart = pos;
            // Try to parse, if can't parse don't save, because string can only be empty, and emty string will be reset to default on focus loss.
            if (uint.TryParse(TBoxAssSeed.Text, out uint seed) == true && seed != SettingsCurrent.Assignment.Seed)
            {
                Settings_Assignment_Seed_Save(seed);
            }
        }
        private void TBoxAssSeed_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxAssSeed.Text == "")
            {
                Settings_Assignment_Seed_Save(SettingsCurrent.Assignment.Seed);
                //
                TBoxAssSeed.Text = SettingsCurrent.Assignment.Seed.ToString();
            }
        }
        private void ButtonAssSeed_Click(object sender, RoutedEventArgs e)
        {
            uint seed = Seed_Generate();
            // Check to avoid double trigger.
            if (seed != SettingsCurrent.Assignment.Seed)
            {
                Settings_Assignment_Seed_Save(seed);
                //
                TBoxAssSeed.Text = seed.ToString();
            }
        }
        // Tab Settings.
        private void TBoxPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_Int32(TBoxPosX.Text, TBoxPosX.SelectionStart, out string str, out int pos);
            TBoxPosX.Text = str;
            TBoxPosX.SelectionStart = pos;
        }
        private void TBoxPosX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxPosX.Text == "" || TBoxPosX.Text == "-")
            {
                TBoxPosX.Text = SettingsCurrent.Settings.PositionX.ToString();
            }
        }
        private void TBoxPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_Int32(TBoxPosY.Text, TBoxPosY.SelectionStart, out string str, out int pos);
            TBoxPosY.Text = str;
            TBoxPosY.SelectionStart = pos;
        }
        private void TBoxPosY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxPosY.Text == "" || TBoxPosY.Text == "-")
            {
                TBoxPosY.Text = SettingsCurrent.Settings.PositionY.ToString();
            }
        }
        private void ButtonPositionApply_Click(object sender, RoutedEventArgs e)
        {
            double positionX = double.Parse(TBoxPosX.Text);
            double positionY = double.Parse(TBoxPosY.Text);
            // Check to avoid double trigger.
            if (positionX != SettingsCurrent.Settings.PositionX || positionY != SettingsCurrent.Settings.PositionY)
            {
                if (positionX >= 0 && positionX <= SystemParameters.VirtualScreenWidth && positionY >= 0 && positionY <= SystemParameters.VirtualScreenHeight)
                {
                    Settings_Settings_PositionX_Save(positionX);
                    Settings_Settings_PositionY_Save(positionY);
                    //
                    Tab_Settings_Position(positionX, positionY);
                }
                else
                {
                    string title = this.Title;
                    string message = "Defined position is off the screen. Do you still want to apply it?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Settings_Settings_PositionX_Save(positionX);
                        Settings_Settings_PositionY_Save(positionY);
                        //
                        Tab_Settings_Position(positionX, positionY);
                    }
                    else
                    {
                        TBoxPosX.Text = SettingsCurrent.Settings.PositionX.ToString();
                        TBoxPosY.Text = SettingsCurrent.Settings.PositionY.ToString();
                    }
                }
            }
        }
        private void TBoxSizeX_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxSizeX.Text, TBoxSizeX.SelectionStart, out string str, out int pos);
            TBoxSizeX.Text = str;
            TBoxSizeX.SelectionStart = pos;
        }
        private void TBoxSizeX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSizeX.Text == "")
            {
                TBoxSizeX.Text = SettingsCurrent.Settings.SizeX.ToString();
            }
        }
        private void TBoxSizeY_TextChanged(object sender, TextChangedEventArgs e)
        {
            String_ReduceTo_UInt32(TBoxSizeY.Text, TBoxSizeY.SelectionStart, out string str, out int pos);
            TBoxSizeY.Text = str;
            TBoxSizeY.SelectionStart = pos;
        }
        private void TBoxSizeY_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSizeY.Text == "")
            {
                TBoxSizeY.Text = SettingsCurrent.Settings.SizeY.ToString();
            }
        }
        private void ButtonSizeApply_Click(object sender, RoutedEventArgs e)
        {
            double sizeX = double.Parse(TBoxSizeX.Text);
            double sizeY = double.Parse(TBoxSizeY.Text);
            // Check to avoid double trigger.
            if (sizeX != SettingsCurrent.Settings.SizeX || sizeY != SettingsCurrent.Settings.SizeY)
            {
                if (sizeX <= SystemParameters.VirtualScreenWidth && sizeY <= SystemParameters.VirtualScreenHeight)
                {
                    Settings_Settings_SizeX_Save(sizeX);
                    Settings_Settings_SizeY_Save(sizeY);
                    //
                    Tab_Settings_Size(sizeX, sizeY);
                }
                else
                {
                    string title = this.Title;
                    string message = "Defined size is bigger than the screen. Do you still want to apply it?";
                    MessageBoxResult result = System.Windows.MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        Settings_Settings_SizeX_Save(sizeX);
                        Settings_Settings_SizeY_Save(sizeY);
                        //
                        Tab_Settings_Size(sizeX, sizeY);
                    }
                    else
                    {
                        TBoxSizeX.Text = SettingsCurrent.Settings.SizeX.ToString();
                        TBoxSizeY.Text = SettingsCurrent.Settings.SizeY.ToString();
                    }
                }
            }
        }
        private void CBoxHideNotes_Click(object sender, RoutedEventArgs e)
        {
            bool hideNotes = CBoxHideNotes.IsChecked.GetValueOrDefault();
            // Check to avoid double trigger.
            if (hideNotes != SettingsCurrent.Settings.HideNotes)
            {
                Settings_Settings_HideNotes_Save(hideNotes);
                //
                Tab_Settings_HideNotes(hideNotes);
            }
        }
        private void TimerWindowPosition_Tick(object sender, EventArgs e)
        {
            double positionX = this.Left;
            double positionY = this.Top;
            //
            Settings_Settings_PositionX_Save(positionX);
            Settings_Settings_PositionY_Save(positionY);
            //
            TBoxPosX.Text = positionX.ToString();
            TBoxPosY.Text = positionY.ToString();
            Timer_WindowPosition_Stop();
        }
        private void TimerWindowSize_Tick(object sender, EventArgs e)
        {
            double sizeX = this.ActualWidth;
            double sizeY = this.ActualHeight;
            //
            Settings_Settings_SizeX_Save(sizeX);
            Settings_Settings_SizeY_Save(sizeY);
            //
            TBoxSizeX.Text = sizeX.ToString();
            TBoxSizeY.Text = sizeY.ToString();
            //
            Timer_WindowSize_Stop();
        }
        #endregion



        public void DeepDivesCustom_Add()
        {
            string filePath = Path.Combine(PathFolderDataDeepDivesStandard, @"0.ini");
            //File.Create(filePath);

            FileINI newFileINI = new FileINI(filePath);
            newFileINI.Write("Seed", TBoxDDSeed.Text, "DeepDive");
            newFileINI.Write("GameVersion", SettingsCurrent.Game.SelectedIdFake, "DeepDive");
            newFileINI.Write("Event", SettingsCurrent.Event.SelectedIdFake, "DeepDive");
            newFileINI.Write("Date", Data.DeepDives[SettingsCurrent.DeepDive.SelectedIdReal].Date, "DeepDive");
            // Normal.
            newFileINI.Write("NormalTag", TBoxDDNorTag.Text, "DeepDive");
            newFileINI.Write("NormalName", TBoxDDNorName.Text, "DeepDive");
            newFileINI.Write("NormalRegion", CBoxDDNorReg.Text, "DeepDive");
            newFileINI.Write("NormalMissionType1", CBoxDDNorMisT1.Text, "DeepDive");
            newFileINI.Write("NormalMissionType2", CBoxDDNorMisT2.Text, "DeepDive");
            newFileINI.Write("NormalMissionType3", CBoxDDNorMisT3.Text, "DeepDive");
            newFileINI.Write("NormalMissionValue1", TBoxDDNorMisV1.Text, "DeepDive");
            newFileINI.Write("NormalMissionValue2", TBoxDDNorMisV2.Text, "DeepDive");
            newFileINI.Write("NormalMissionValue3", TBoxDDNorMisV3.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveType1", CBoxDDNorObjT1.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveType2", CBoxDDNorObjT2.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveType3", CBoxDDNorObjT3.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveValue1", TBoxDDNorObjV1.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveValue2", TBoxDDNorObjV2.Text, "DeepDive");
            newFileINI.Write("NormalObjectiveValue3", TBoxDDNorObjV3.Text, "DeepDive");
            newFileINI.Write("NormalWarning1", CBoxDDNorWar1.Text, "DeepDive");
            newFileINI.Write("NormalWarning2", CBoxDDNorWar2.Text, "DeepDive");
            newFileINI.Write("NormalWarning3", CBoxDDNorWar3.Text, "DeepDive");
            newFileINI.Write("NormalAnomaly1", CBoxDDNorAno1.Text, "DeepDive");
            newFileINI.Write("NormalAnomaly2", CBoxDDNorAno2.Text, "DeepDive");
            newFileINI.Write("NormalAnomaly3", CBoxDDNorAno3.Text, "DeepDive");
            // Elite.
            newFileINI.Write("EliteTag", TBoxDDEliTag.Text, "DeepDive");
            newFileINI.Write("EliteName", TBoxDDEliName.Text, "DeepDive");
            newFileINI.Write("EliteRegion", CBoxDDEliReg.Text, "DeepDive");
            newFileINI.Write("EliteMissionType1", CBoxDDEliMisT1.Text, "DeepDive");
            newFileINI.Write("EliteMissionType2", CBoxDDEliMisT2.Text, "DeepDive");
            newFileINI.Write("EliteMissionType3", CBoxDDEliMisT3.Text, "DeepDive");
            newFileINI.Write("EliteMissionValue1", TBoxDDEliMisV1.Text, "DeepDive");
            newFileINI.Write("EliteMissionValue2", TBoxDDEliMisV2.Text, "DeepDive");
            newFileINI.Write("EliteMissionValue3", TBoxDDEliMisV3.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveType1", CBoxDDEliObjT1.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveType2", CBoxDDEliObjT2.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveType3", CBoxDDEliObjT3.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveValue1", TBoxDDEliObjV1.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveValue2", TBoxDDEliObjV2.Text, "DeepDive");
            newFileINI.Write("EliteObjectiveValue3", TBoxDDEliObjV3.Text, "DeepDive");
            newFileINI.Write("EliteWarning1", CBoxDDEliWar1.Text, "DeepDive");
            newFileINI.Write("EliteWarning2", CBoxDDEliWar2.Text, "DeepDive");
            newFileINI.Write("EliteWarning3", CBoxDDEliWar3.Text, "DeepDive");
            newFileINI.Write("EliteAnomaly1", CBoxDDEliAno1.Text, "DeepDive");
            newFileINI.Write("EliteAnomaly2", CBoxDDEliAno2.Text, "DeepDive");
            newFileINI.Write("EliteAnomaly3", CBoxDDEliAno3.Text, "DeepDive");
        }

        private void ButtonDeepDiveAdd_Click(object sender, RoutedEventArgs e)
        {
            DeepDivesCustom_Add();
        }
    }
    public class ItemGameVersion
    {
        public string IdFake {get; set;}
        public string Name {get; set;}
        public string Year {get; set;}
        public Brush Brush {get; set;}
    }
    public class ItemDeepDive
    {
        public string IdFake { get; set; }
        public string Date { get; set; }
        public string NameNormal { get; set; }
        public string NameElite { get; set; }
        public Brush Brush { get; set; }
    }
    public class ItemEvent
    {
        public string IdFake { get; set; }
        public string Year { get; set; }
        public string Name { get; set; }
        public Brush Brush { get; set; }
    }
    public class ItemEventItem
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
