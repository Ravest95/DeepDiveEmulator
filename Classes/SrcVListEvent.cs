using System.Windows.Media;

namespace DeepDiveEmulator.Classes
{
    public class SrcVListEvent
    {
        private string _Number = "";
        private string _Date = "";
        private string _Name = "";
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public string Number { get { return _Number; } set { _Number = value; } }
        public string Date { get { return _Date; } set { _Date = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
    }
}
