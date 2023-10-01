using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class DataDiveParam
    {
        private List<string> _Anomalies = new List<string>();
        private List<DataDiveParamBrush> _Brushes = new List<DataDiveParamBrush>();
        private List<string> _Missions = new List<string>();
        private List<string> _Objectives = new List<string>();
        private List<string> _Regions = new List<string>();
        private List<string> _Warnings = new List<string>();

        public List<string> Anomalies { get { return _Anomalies; } set { _Anomalies = value; } }
        public List<DataDiveParamBrush> Brushes { get { return _Brushes; } set { _Brushes = value; } }
        public List<string> Missions { get { return _Missions; } set { _Missions = value; } }
        public List<string> Objectives { get { return _Objectives; } set { _Objectives = value; } }
        public List<string> Regions { get { return _Regions; } set { _Regions = value; } }
        public List<string> Warnings { get { return _Warnings; } set { _Warnings = value; } }
    }
}
