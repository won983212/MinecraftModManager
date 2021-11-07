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
            wnd.Width = 800;
            wnd.Height = 450;
            wnd.Content = viewModel;
            wnd.Show();
        }
    }
}
