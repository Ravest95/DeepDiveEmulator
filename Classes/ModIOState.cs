using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class ModIOState
    {
        private List<ModIOStateMod> _Mods = new List<ModIOStateMod>();
        private int _version = 1;

        public List<ModIOStateMod> Mods { get { return _Mods; } set { _Mods = value; } }
        public int version { get { return _version; } set { _version = value; } }
    }
}
