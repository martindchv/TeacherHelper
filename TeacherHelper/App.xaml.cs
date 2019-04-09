using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace TeacherHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.LoadCompleted += App_LoadCompleted;
        }

        public NavigationService Navigation { get; private set; }
        public new static App Current
        {
            get
            {
                return Application.Current as App;
            }
        }

        private void App_LoadCompleted(object sender, NavigationEventArgs e)
        {
            Navigation = (App.Current.MainWindow as MainWindow).mainFrame.NavigationService;
        }

        
    }
}
