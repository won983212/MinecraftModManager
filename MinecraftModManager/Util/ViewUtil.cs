using MinecraftModManager.View;
using MinecraftModManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftModManager.Util
{
    public static class ViewUtil
    {
        public static void ShowWindow(object viewModel)
        {
            Window wnd = new Window();
            wnd.SizeToContent = SizeToContent.Width;
            wnd.Content = viewModel;
            wnd.Show();
        }

        public static string ShowInputDialog(string message, string defaultValue = "")
        {
            InputDialog dialog = new InputDialog();
            dialog.MessageTbx.Text = message;
            dialog.InputTbx.Text = defaultValue;
            if (dialog.ShowDialog() == true)
                return dialog.InputTbx.Text;
            return "";
        }
    }
}
