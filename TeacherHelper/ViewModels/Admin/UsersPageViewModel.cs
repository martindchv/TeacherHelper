using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Services.Services.Admin;

namespace TeacherHelper.ViewModels.Admin
{
    public class UsersPageViewModel : INotifyPropertyChanged
    {
        private List<UserDTONotify> allUsers;
        private ObservableCollection<UserDTONotify> selectedUsers;
        private string filter;
        private UserDTONotify currentUser;
        private List<SubjectDTO> allSubjects;
        private ObservableCollection<SubjectDTO> subjectsToShow;
        private UsersService service;

        public UsersPageViewModel()
        {
            this.service = new UsersService();
            this.allUsers = this.service.LoadAllUsers().Select(u => new UserDTONotify(u)).ToList();
            this.allSubjects = this.service.LoadSubjects();
            this.UsersToDisplay = new ObservableCollection<UserDTONotify>(this.allUsers);
        }

        public ObservableCollection<UserDTONotify> UsersToDisplay
        {
            get { return this.selectedUsers; }
            set
            {
                this.selectedUsers = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<SubjectDTO> SubjectsToShow
        {
            get { return this.subjectsToShow; }
            set
            {
                this.subjectsToShow = value;
                this.RaisePropertyChanged();
            }
        }

        public SubjectDTO SelectedSubject
        {
            get { return null; }
            set
            {
                this.CurrentUser.Subjects.Add(value);
                this.SubjectsToShow.Remove(value);
            }
        }

        public UserDTONotify CurrentUser
        {
            get { return this.currentUser; }
            set
            {
                this.currentUser = value;

                if (value != null)
                {
                    this.SubjectsToShow = new ObservableCollection<SubjectDTO>(this.allSubjects.Where(s => !value.Subjects.Any(us => us.Id == s.Id)));
                }
                else
                {
                    this.SubjectsToShow = new ObservableCollection<SubjectDTO>(this.allSubjects);
                }

                this.RaisePropertyChanged();
            }
        }

        public ICommand SearchUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!string.IsNullOrWhiteSpace(this.SearchText))
                    {
                        this.filter = this.SearchText.ToLower();
                    }
                    else
                    {
                        this.filter = string.Empty;
                    }
                    
                    this.UsersToDisplay.Clear();

                    this.allUsers.Where(u => u.Username.ToLower().IndexOf(this.filter) != -1 
                                       || u.Name.ToLower().Contains(this.filter))
                                 .ToList()
                                 .ForEach(this.UsersToDisplay.Add);
                });
            }
        }

        public ICommand DeleteUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!ConfirmationWindow.ConfirmDelete())
                    {
                        return;
                    }

                    var user = param as UserDTONotify;
                    user.IsActive = false;
                    this.service.DeleteUser(user.Id);
                });
            }
        }

        public ICommand RestoreUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var user = param as UserDTONotify;
                    user.IsActive = true;
                    this.service.RestoreUser(user.Id);
                });
            }
        }

        public ICommand CreateEditUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentUser != null && !ConfirmationWindow.ConfirmOverride())
                    {
                        return;
                    }

                    if (param == null)
                    {
                        this.CurrentUser = new UserDTONotify();
                    }
                    else
                    {
                        this.CurrentUser = new UserDTONotify(param as UserDTONotify);
                    }
                });
            }
        }

        public ICommand CancelEditCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!ConfirmationWindow.ConfirmCancel())
                    {
                        return;
                    }

                    this.CurrentUser = null;
                });
            }
        }

        public ICommand RemoveSubjectFromUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.CurrentUser.Subjects.Remove(param as SubjectDTO);
                });
            }
        }

        public ICommand SaveUserCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (string.IsNullOrWhiteSpace(this.CurrentUser.Email) 
                    || string.IsNullOrWhiteSpace(this.CurrentUser.Name)
                    || string.IsNullOrWhiteSpace(this.CurrentUser.Username)
                    || this.CurrentUser.Subjects.Count == 0)
                    {
                        MessageBox.Show("Моля попълнете всички полета!", "Внимание");
                        return;
                    }

                    var regex = new Regex(@"\w+?@\w+?\.\w+");

                    if (!regex.IsMatch(this.CurrentUser.Email))
                    {
                        MessageBox.Show("Моля въведете валиден имейл адрес!");
                        return;
                    }

                    if (this.CurrentUser.Id != 0)
                    {
                        var user = this.allUsers.First(u => u.Id == this.CurrentUser.Id);

                        if (!this.CurrentUser.Subjects.Any(s => s.Id == this.CurrentUser.Subject.Id))
                        {
                            this.CurrentUser.Subject = this.CurrentUser.Subjects.First();
                        }

                        user.CopyUser(this.CurrentUser);
                        this.service.UpdateUser(this.CurrentUser as UserDTO);

                        if (this.CurrentUser.Id == Authenticator.CurrentUser.Id)
                        {
                            Authenticator.CurrentUser.Name = this.CurrentUser.Name;
                            Authenticator.CurrentUser.Username = this.CurrentUser.Username;
                            Authenticator.CurrentUser.Subject.Id = this.CurrentUser.Subject.Id;
                            Authenticator.CurrentUser.Subject.Name = this.CurrentUser.Subject.Name;
                            Authenticator.UpdateUser(Authenticator.CurrentUser);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(this.CurrentUser.Password))
                        {
                            MessageBox.Show("Моля попълнете всички полета!", "Внимание");
                            return;
                        }

                        this.CurrentUser.IsActive = true;

                        this.allUsers.Add(this.CurrentUser);
                        if (string.IsNullOrWhiteSpace(this.filter)
                        || !string.IsNullOrWhiteSpace(this.filter)
                            && (this.CurrentUser.Username.ToLower().Contains(this.filter)
                            || this.CurrentUser.Name.ToLower().Contains(this.filter)))
                        {
                            this.UsersToDisplay.Add(this.CurrentUser);
                        }

                        this.CurrentUser.Subject = this.CurrentUser.Subjects.First();

                        this.service.RegisterUser(this.CurrentUser);
                    }

                    this.CurrentUser = null;
                });
            }
        }

        private string searchText;
        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                this.searchText = value;
                this.RaisePropertyChanged();
            }
        }

        private object padlock = new object();
        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (this.padlock)
                {
                    propertyChanged += value;
                }
            }
            remove
            {
                lock (this.padlock)
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
    }

    public class UserDTONotify : UserDTO, INotifyPropertyChanged
    {
        public UserDTONotify()
        {
            base.Subjects = new ObservableCollection<SubjectDTO>();
        }

        public UserDTONotify(UserDTO original)
        {
            this.CopyUser(original);
        }

        public new bool IsActive
        {
            get { return base.IsActive; }
            set
            {
                base.IsActive = value;
                this.RaisePropertyChanged();
            }
        }

        public new string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public new string Username
        {
            get { return base.Username; }
            set
            {
                base.Username = value;
                this.RaisePropertyChanged();
            }
        }

        public new string Email
        {
            get { return base.Email; }
            set
            {
                base.Email = value;
                this.RaisePropertyChanged();
            }
        }

        public new bool IsAdmin
        {
            get { return base.IsAdmin; }
            set
            {
                base.IsAdmin = value;
                this.RaisePropertyChanged();
            }
        }

        public void CopyUser(UserDTO original)
        {
            base.Id = original.Id;
            this.Email = original.Email;
            this.Name = original.Name;
            this.IsActive = original.IsActive;
            this.IsAdmin = original.IsAdmin;
            this.Username = original.Username;
            base.ProfilePicture = original.ProfilePicture;
            base.Subjects = original.Subjects;
            base.Subject = original.Subject;
        }

        private object padlock = new object();
        private event PropertyChangedEventHandler propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (this.padlock)
                {
                    propertyChanged += value;
                }
            }
            remove
            {
                lock (this.padlock)
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
    }
}
