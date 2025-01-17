using System.ComponentModel;

namespace RpdPlayerApp.Models
{
    class CustomChipModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private bool isSelected = false;

        public event PropertyChangedEventHandler PropertyChanged;

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
