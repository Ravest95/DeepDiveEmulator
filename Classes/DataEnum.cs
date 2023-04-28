using System.Collections.Generic;
using System.Windows.Media;

namespace DeepDiveEmulator.ClassDataEnum
{
    public class DataEnum
    {
        public List<string> Regions = new List<string>();
        public List<string> Missions = new List<string>();
        public List<string> SideObjectives = new List<string>();
        public List<string> Warnings = new List<string>();
        public List<string> Anomalies = new List<string>();
        public List<DataEnumBrush> Brushes = new List<DataEnumBrush>();
    }
    public class DataEnumBrush
    {
        public string Name = "";
        public Brush Brush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
    }
}
