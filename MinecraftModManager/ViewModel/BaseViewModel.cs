using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinecraftModManager.ViewModel
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetPropertyAndNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            OnPropertyChanged(propertyName);
        }
    }
}
