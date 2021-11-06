using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftModManager.Util
{
    public static class FileSystemUtil
    {
        public delegate void FilterSelector(CommonFileDialogFilterCollection filters);

        public static string SelectFile(string title, FilterSelector filterSelector = null, string initialPath = "C:/users")
        {
            return SelectFileWithDialog(title, filterSelector, initialPath, false);
        }

        public static string SelectDirectory(string title, FilterSelector filterSelector = null, string initialPath = "C:/users")
        {
            return SelectFileWithDialog(title, filterSelector, initialPath, true);
        }

        private static string SelectFileWithDialog(string title, FilterSelector filterSelector, string initialPath, bool isDir)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Title = title;
            dialog.InitialDirectory = initialPath;
            dialog.IsFolderPicker = isDir;

            if (filterSelector != null)
                filterSelector(dialog.Filters);

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok || string.IsNullOrWhiteSpace(dialog.FileName))
                return null;

            return dialog.FileName;
        }
    }
}
