namespace DeepDiveEmulator.Classes
{
    public class ModIOStateMod
    {
        private int _ID = 0;
        private string _PathOnDisk = "";
        private ModIOStateProfile _Profile = new ModIOStateProfile();
        private int _State = 1;

        public int ID { get { return _ID; } set { _ID = value; } }
        public string PathOnDisk { get { return _PathOnDisk; } set { _PathOnDisk = value; } }
        public ModIOStateProfile Profile { get { return _Profile; } set { _Profile = value; } }
        public int State { get { return _State; } set { _State = value; } }
    }
}
