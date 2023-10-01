namespace DeepDiveEmulator.Classes
{
    public class ModIOGlobalSettings
    {
        private string _RootLocalStoragePath = "";

        public string RootLocalStoragePath { get { return _RootLocalStoragePath; } set { _RootLocalStoragePath = value; } }
    }
}
