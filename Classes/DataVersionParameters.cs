﻿using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class DataVersionParameters
    {
        private List<string> _Branches = new List<string>();

        public List<string> Branches { get { return _Branches; } set { _Branches = value; } }
    }
}
