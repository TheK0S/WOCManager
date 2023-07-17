using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WOCManager.View;

namespace WOCManager.Controller
{
    internal class MainWindowController
    {
        private RelayCommand? _addCategoryWindow;
        private RelayCommand? _addWordWindow;
        private RelayCommand? _minimizeWindow;
        private RelayCommand? _closeWindow;

        public RelayCommand AddCategoryWindow
        {
            get
            {
                return _addCategoryWindow ??
                    (_addCategoryWindow = new RelayCommand(async obj =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() => { new CategoryWindow().Show(); });
                    }));
            }
        }

        public RelayCommand AddWordWindow
        {
            get
            {
                return _addWordWindow ??
                    (_addWordWindow = new RelayCommand(async obj =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() => { new WordWindow().Show(); });
                    }));
            }
        }

        public RelayCommand? MinimizeWindow
        {
            get
            {
                return _minimizeWindow ??
                    (_minimizeWindow = new RelayCommand(obj =>
                    {
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    }));                
            }
        }
        public RelayCommand? CloseWindow
        {
            get
            {
                return _closeWindow ??
                    (_closeWindow = new RelayCommand(obj =>
                    {
                        Application.Current.MainWindow.Close();
                    }));                
            }
        }
    }
}
