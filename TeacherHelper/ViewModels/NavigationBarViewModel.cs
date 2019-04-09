using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Common;

namespace TeacherHelper.ViewModels
{
    public class NavigationBarViewModel : INotifyPropertyChanged
    {
        private const int adminMenuBorderSize = 3;
        private const int userMenuBorderSize = 2;

        private ObservableCollection<NavigationItem> navigationItems;
        private bool menuToggled;

        public NavigationBarViewModel()
        {
            UpdateNavigation();
        }

        public ObservableCollection<NavigationItem> NavigationItems
        {
            get { return navigationItems; }
            set
            {
                navigationItems = value;
                RaisePropertyChanged();
            }
        }

        public UserDTO CurrentUser
        {
            get { return Authenticator.CurrentUser; }
        }

        public int MenuBorderRowSpan
        {
            get { return (CurrentUser != null && CurrentUser.IsAdmin) ? adminMenuBorderSize : userMenuBorderSize; }
        }

        public void UpdateUserInfo()
        {
            UpdateNavigation();
            RaisePropertyChanged(nameof(CurrentUser));
        }

        private void UpdateNavigation()
        {
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { Name = "Тестове", Path = "Pages/TestsPage.xaml", ShouldDisplay = true },
                new NavigationItem { Name = "Въпроси", Path = "Pages/QuestionsPage.xaml", ShouldDisplay = true },
                new NavigationItem { Name = "Календар", Path = "Pages/CalendarPage.xaml", ShouldDisplay = true },
                new NavigationItem { Name = "Адм. потребители", Path = "Pages/Admin/UsersPage.xaml", ShouldDisplay = CurrentUser != null && CurrentUser.IsAdmin },
                new NavigationItem { Name = "Адм. предмети", Path = "Pages/Admin/SubjectsPage.xaml", ShouldDisplay = CurrentUser != null && CurrentUser.IsAdmin }
            };

            RaisePropertyChanged(nameof(MenuBorderRowSpan));
        }

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private object padlock = new object();

        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (padlock)
                {
                    propertyChanged += value;
                }
            }
            remove
            {
                lock (padlock)
                {
                    propertyChanged -= value;
                }

            }
        }


        public ICommand ToggleMenuCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    MenuToggled = !MenuToggled;
                });
            }
        }

        public ICommand NavigateToCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var navigateTo = (param as NavigationItem).Path;

                    App.Current.Navigation.Navigate(new Uri(navigateTo, UriKind.Relative));
                });
            }
        }

        public bool MenuToggled
        {
            get { return this.menuToggled; }
            set
            {
                this.menuToggled = value;
                RaisePropertyChanged();
            }
        }
    }

    public class NavigationItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool ShouldDisplay { get; set; }
    }
}
