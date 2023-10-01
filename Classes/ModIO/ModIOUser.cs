using System.Collections.Generic;

namespace DeepDiveEmulator.Classes
{
    public class ModIOUser
    {
        public ModIOEmpty _Avatar = new ModIOEmpty();
        public ModIOEmpty _OAuth = new ModIOEmpty();
        public ModIOEmpty _Profile = new ModIOEmpty();
        public List<int> _subscriptions = new List<int>();

        public ModIOEmpty Avatar { get { return _Avatar; } set { _Avatar = value; } }
        public ModIOEmpty OAuth { get { return _OAuth; } set { _OAuth = value; } }
        public ModIOEmpty Profile { get { return _Profile; } set { _Profile = value; } }
        public List<int> subscriptions { get { return _subscriptions; } set { _subscriptions = value; } }
    }
}
