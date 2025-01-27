using System.ComponentModel;

namespace RpdPlayerApp.Architecture
{
    class CustomChipModel : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;

        private bool isSelected = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }
    }
}
