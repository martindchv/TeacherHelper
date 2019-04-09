using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Tests;
using TeacherHelper.Models.Enums;
using TeacherHelper.Pages;
using TeacherHelper.Services.Services;

namespace TeacherHelper.ViewModels
{
    public class TestsPageViewModel : INotifyPropertyChanged
    {
        private TestService service;
        public TestsPageViewModel()
        {
            this.service = new TestService();
            this.UserTests = this.service.GetUserTests(Authenticator.CurrentUser.Id);
            this.CurrentThreeTests = new ObservableCollection<TestDTO>(UserTests.Take(3));
        }

        private ObservableCollection<TestDTO> currentThreeTests = new ObservableCollection<TestDTO>();
        private List<TestDTO> userTests = new List<TestDTO>();

        public List<TestDTO> UserTests
        {
            get { return this.userTests; }
            set
            {
                this.userTests = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<TestDTO> CurrentThreeTests
        {
            get { return this.currentThreeTests; }
            set
            {
                this.currentThreeTests = value;
                RaisePropertyChanged();
            }
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

        private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public ICommand MoveTestsLeftCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentThreeTests.Count <= 1)
                    {
                        return;
                    }

                    this.CurrentThreeTests.Remove(this.CurrentThreeTests.Last());

                    var indexToAdd = this.UserTests.IndexOf(this.CurrentThreeTests.First());

                    if (indexToAdd == 0)
                    {
                        indexToAdd = this.UserTests.Count - 1;
                    }
                    else
                    {
                        indexToAdd--;
                    }

                    this.CurrentThreeTests.Insert(0, this.UserTests[indexToAdd]);
                });
            }
        }

        public ICommand MoveTestsRightCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentThreeTests.Count <= 1)
                    {
                        return;
                    }

                    this.CurrentThreeTests.Remove(this.CurrentThreeTests.First());

                    var indexToAdd = this.UserTests.IndexOf(this.CurrentThreeTests.Last());

                    if (indexToAdd == this.UserTests.Count - 1)
                    {
                        indexToAdd = 0;
                    }
                    else
                    {
                        indexToAdd++;
                    }

                    this.CurrentThreeTests.Add(this.UserTests[indexToAdd]);
                });
            }
        }

        public ICommand CreateTestCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    App.Current.Navigation.Navigate(new TestGenerationPage());
                });
            }
        }

        public ICommand ViewTestCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var test = param as TestDTO;
                    App.Current.Navigation.Navigate(new TestGenerationPage(test.Id));
                });
            }
        }
    }
}
