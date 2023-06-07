using System.Windows.Media;

namespace DeepDiveEmulator.Classes
{
    public class SrcCBoxBrush
    {
        private string _Name = "";
        private Brush _Brush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));

        public string Name { get { return _Name; } set { _Name = value; } }
        public Brush Brush { get { return _Brush; } set { _Brush = value; } }
    }
}
