using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Services.Services;

namespace TeacherHelper.ViewModels
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        private AuthenticationService service;
        private string username;
        private string password;

        public LoginPageViewModel()
        {
            this.service = new AuthenticationService();
            this.Username = string.Empty;
            this.Password = string.Empty;
        }

        public string Username
        {
            get { return this.username; }
            set
            {
                this.username = value;
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                this.password = value;
                RaisePropertyChanged();
            }
        }
        
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    try
                    {
                        var user = this.service.Login(this.Username, this.Password);

                        if (user != null)
                        {
                            Authenticator.UpdateUser(user);
                            App.Current.Navigation.Navigate(new Uri("Pages/QuestionsPage.xaml", UriKind.Relative));
                            NavBarHelper.UpdateUserData();
                            NavBarHelper.ShowNavbar();
                        }
                        else
                        {
                            MessageBox.Show($"Invalid username or password.");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Server connection failed.");
                    }
                });
            }
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
    }
}
