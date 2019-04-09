using System.Windows;
using System.Windows.Controls;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.Models;
using TeacherHelper.Pages;
using TeacherHelper.ViewModels;

namespace TeacherHelper.Helpers
{
    public static class NavBarHelper
    {
        private static MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
        private static NavigationBar navBar = mainWindow.navBar.Content as NavigationBar;
        public static void HideNavbar()
        {
            mainWindow.navBar.Visibility = Visibility.Collapsed;
            Grid.SetRow(mainWindow.mainFrame, 0);
            Grid.SetRowSpan(mainWindow.mainFrame, 2);
        }

        public static void ShowNavbar()
        {
            mainWindow.navBar.Visibility = Visibility.Visible;
            Grid.SetRow(mainWindow.mainFrame, 1);
            Grid.SetRowSpan(mainWindow.mainFrame, 1);
        }

        public static void UpdateUserData()
        {
            (navBar.DataContext as NavigationBarViewModel).UpdateUserInfo();
        }
    }
}
