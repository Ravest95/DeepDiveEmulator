using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class DataURLs
    {
        private List<string> _Assignments = new List<string>();
        private List<string> _Dives = new List<string>();
        private List<string> _Events = new List<string>();
        private List<string> _FreeBeers = new List<string>();

        public List<string> Assignments { get { return _Assignments; } set { _Assignments = value; } }
        public List<string> Dives { get { return _Dives; } set { _Dives = value; } }
        public List<string> Events { get { return _Events; } set { _Events = value; } }
        public List<string> FreeBeers { get { return _FreeBeers; } set { _FreeBeers = value; } }
    }
}
