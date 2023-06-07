using System.Windows.Media;

namespace DeepDiveEmulator.Classes
{
    public class SrcVListDive
    {
        private string _Number = "";
        private string _NameNormal = "";
        private string _NameElite = "";
        private string _Date = "";
        private string _Event = "";
        private Brush _BrushBack = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private Brush _BrushBord = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

        public string Number { get { return _Number; } set { _Number = value; } }
        public string NameNormal { get { return _NameNormal; } set { _NameNormal = value; } }
        public string NameElite { get { return _NameElite; } set { _NameElite = value; } }
        public string Date { get { return _Date; } set { _Date = value; } }
        public string Event { get { return _Event; } set { _Event = value; } }
        public Brush BrushBack { get { return _BrushBack; } set { _BrushBack = value; } }
        public Brush BrushBord { get { return _BrushBord; } set { _BrushBord = value; } }
    }
}
