using MinecraftModManager.Model;
using System.Windows;
using System.Windows.Controls;

namespace MinecraftModManager.View.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                ModListbox.Items.Filter = null;
            }
            else
            {
                ModListbox.Items.Filter = (o) => { return ModNameEquals(o, text); };
            }
        }

        private static bool ModNameEquals(object modObj, string name)
        {
            Mod mod = modObj as Mod;
            if (mod == null)
                return false;
            string modName = mod.Name.ToLower().Replace(" ", "");
            string valueName = name.ToLower().Replace(" ", "");
            return modName.Contains(valueName);
        }
    }
}
