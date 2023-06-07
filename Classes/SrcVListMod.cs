using System.Windows.Media;

namespace DeepDiveEmulator.Classes
{
    public class SrcVListMod
    {
        private string _Number = "";
        private string _Name = "";
        private string _Date = "";
        private bool _IsEnabled = false;
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private Brush _BrushBord = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public string Number { get { return _Number; } set { _Number = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Date { get { return _Date; } set { _Date = value; } }
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
        public Brush BrushBord { get { return _BrushBord; } set { _BrushBord = value; } }
    }
}
