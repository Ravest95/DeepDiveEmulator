using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class ModIOStateProfile
    {
        private List<ModIOStateTag> _tags = new List<ModIOStateTag>()
        {
            new ModIOStateTag(){name = "Optional"},
            new ModIOStateTag(){},
            new ModIOStateTag(){name = "Verified"}
        };

        public List<ModIOStateTag> tags { get { return _tags; } set { _tags = value; } }
    }
}
