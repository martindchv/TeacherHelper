using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Admin.Subjects;
using TeacherHelper.Services.Services.Admin;

namespace TeacherHelper.ViewModels.Admin
{
    public class SubjectsPageViewModel : INotifyPropertyChanged
    {
        private SubjectsService service;
        private ObservableCollection<SubjectDTONotify> subjects;
        private SubjectDTONotify currentSubject;

        public SubjectsPageViewModel()
        {
            this.service = new SubjectsService();
            this.Subjects = new ObservableCollection<SubjectDTONotify>(this.service
                    .GetSubjects()
                    .Select(s => new SubjectDTONotify(s)));
        }

        public ObservableCollection<SubjectDTONotify> Subjects
        {
            get { return this.subjects; }
            set
            {
                this.subjects = value;
                RaisePropertyChanged();
            }
        }

        public SubjectDTONotify CurrentSubject
        {
            get { return this.currentSubject; }
            set
            {
                this.currentSubject = value;
                RaisePropertyChanged();
            }
        }

        public ICommand CreateEditSubjectCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (param == null)
                    {
                        this.CurrentSubject = new SubjectDTONotify();
                    }
                    else
                    {
                        this.CurrentSubject = new SubjectDTONotify(param as SubjectDTO);
                    }
                });
            }
        }

        public ICommand DeleteSubjectCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!ConfirmationWindow.ConfirmDelete())
                    {
                        return;
                    }

                    var subject = param as SubjectDTONotify;

                    this.subjects.Remove(subject);
                    this.service.DeleteSubject(subject);

                    if (this.CurrentSubject != null && this.CurrentSubject.Id == subject.Id)
                    {
                        this.CurrentSubject = null;
                    }
                });
            }
        }

        public ICommand AddThemeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (param == null)
                    {
                        var lastTheme = this.CurrentSubject.Themes.LastOrDefault();
                        
                        if (lastTheme != null)
                        {
                            var themeToAdd = new ThemeDTO
                            {
                                PreviousTheme = lastTheme
                            };

                            lastTheme.NextTheme = themeToAdd;

                            this.CurrentSubject.Themes.Add(themeToAdd);
                        }
                        else
                        {
                            this.CurrentSubject.Themes.Add(new ThemeDTO());
                        }
                    }
                    else
                    {
                        var parentTheme = param as ThemeDTO;
                        var previousTheme = parentTheme.ChildThemes?.LastOrDefault();

                        var theme = new ThemeDTO
                        {
                            PreviousTheme = previousTheme,
                            ParentTheme = parentTheme,
                            TreeDepth = parentTheme.TreeDepth + 1
                        };

                        if (previousTheme != null)
                        {
                            previousTheme.NextTheme = theme;
                        }

                        parentTheme.ChildThemes.Add(theme);
                    }
                });
            }
        }

        public ICommand RemoveThemeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var theme = param as ThemeDTO;

                    if (theme.ParentTheme != null)
                    {
                        theme.ParentTheme.ChildThemes.Remove(theme);
                    }
                    else
                    {
                        this.CurrentSubject.Themes.Remove(theme);
                    }
                });
            }
        }

        public ICommand MoveDownwardsThemeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var theme = param as ThemeDTO;

                    if (theme.NextTheme == null)
                    {
                        return;
                    }

                    if (theme.ParentTheme == null)
                    {
                        var index = this.CurrentSubject.Themes.IndexOf(theme);
                        this.CurrentSubject.Themes[index] = theme.NextTheme;
                        this.CurrentSubject.Themes[index + 1] = theme;
                    }
                    else
                    {
                        var index = theme.ParentTheme.ChildThemes.IndexOf(theme);
                        theme.ParentTheme.ChildThemes[index] = theme.NextTheme;
                        theme.ParentTheme.ChildThemes[index + 1] = theme;
                    }

                    // Workaround, TODO refactor
                    var a = theme.PreviousTheme;
                    var b = theme;
                    var c = theme.NextTheme;
                    var d = theme.NextTheme.NextTheme;

                    c.PreviousTheme = a;
                    c.NextTheme = b;
                    b.PreviousTheme = c;
                    b.NextTheme = d;
                });
            }
        }

        public ICommand MoveUpwardsThemeCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    var theme = param as ThemeDTO;

                    if (theme.PreviousTheme == null)
                    {
                        return;
                    }

                    if (theme.ParentTheme == null)
                    {
                        var index = this.CurrentSubject.Themes.IndexOf(theme);
                        this.CurrentSubject.Themes[index] = theme.PreviousTheme;
                        this.CurrentSubject.Themes[index - 1] = theme;
                    }
                    else
                    {
                        var index = theme.ParentTheme.ChildThemes.IndexOf(theme);
                        theme.ParentTheme.ChildThemes[index] = theme.PreviousTheme;
                        theme.ParentTheme.ChildThemes[index - 1] = theme;
                    }

                    // Workaround, TODO refactor
                    var a = theme.PreviousTheme.PreviousTheme;
                    var b = theme.PreviousTheme;
                    var c = theme;
                    var d = theme.NextTheme;

                    c.PreviousTheme = a;
                    c.NextTheme = b;
                    b.PreviousTheme = c;
                    b.NextTheme = d;
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

                    this.CurrentSubject = null;
                });
            }
        }

        public ICommand SaveSubjectCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentSubject.Id != 0)
                    {
                        this.service.UpdateSubject(this.CurrentSubject);
                        var subj = this.Subjects.FirstOrDefault(s => s.Id == this.CurrentSubject.Id);
                        subj.Copy(this.CurrentSubject);
                    }
                    else
                    {
                        this.service.CreateSubject(this.CurrentSubject);
                        this.Subjects.Add(this.CurrentSubject);
                    }

                    this.CurrentSubject = null;
                });
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

    public class SubjectDTONotify : SubjectDTO, INotifyPropertyChanged
    {
        public SubjectDTONotify(SubjectDTO param)
        {
            this.Copy(param);
        }

        public SubjectDTONotify()
        {
            base.Themes = new ObservableCollection<ThemeDTO>();
        }

        public new string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                RaisePropertyChanged();
            }
        }

        public void Copy(SubjectDTO subject)
        {
            base.Id = subject.Id;
            this.Name = subject.Name;
            base.Themes = subject.Themes;
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
