using System.ComponentModel;
using System.Threading.Tasks;

namespace Channeler.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
