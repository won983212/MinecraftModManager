using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace MinecraftModManager.Model
{
    public class Mod : INotifyPropertyChanged
    {
        private string _name = "";
        private string _version = "";
        private string _description = "";
        private string _filePath = "";
        private BitmapImage _thumbnail;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set => SetPropertyAndNotify(ref _name, value);
        }

        public string Version
        {
            get => _version;
            set => SetPropertyAndNotify(ref _version, value);
        }

        public string Description
        {
            get => _description;
            set => SetPropertyAndNotify(ref _description, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetPropertyAndNotify(ref _filePath, value);
        }

        public BitmapImage Thumbnail
        {
            get => _thumbnail;
            set => SetPropertyAndNotify(ref _thumbnail, value);
        }

        public string LogoPath { get; set; }


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
