using System.Windows;
using TeacherHelper.Helpers;
using TeacherHelper.Services.Services;

namespace TeacherHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DatabaseConnection.Try())
            {
                MessageBox.Show("Failed to connect to server database. Shutting down application...");
                Application.Current.Shutdown();
            }

            NavBarHelper.HideNavbar();
        }
    }
}
