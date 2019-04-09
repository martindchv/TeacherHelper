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
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.DTOs.QuestionsPage;
using TeacherHelper.Models.Enums;
using TeacherHelper.Services.Services;

namespace TeacherHelper.ViewModels
{
    public class QuestionsViewModel : INotifyPropertyChanged
    {
        private QuestionsService service;
        public QuestionsViewModel()
        {
            this.service = new QuestionsService();

            this.UserQuestions = new ObservableCollection<QuestionDTO>(this.service.GetUserQuestions(Authenticator.CurrentUser.Id, Authenticator.CurrentUser.Subject.Id));
            this.Themes = this.service.LoadThemes(Authenticator.CurrentUser.Subject.Id);
        }

        private QuestionDTO currentQuestion;
        private List<ThemeDTO> themes;
        private ObservableCollection<QuestionDTO> userQuestions;

        public List<ThemeDTO> Themes
        {
            get { return this.themes; }
            set
            {
                this.themes = value.ToList();
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<QuestionDTO> UserQuestions
        {
            get { return this.userQuestions; }
            set
            {
                this.userQuestions = value;
                RaisePropertyChanged();
            }
        }

        public QuestionDTO CurrentQuestion
        {
            get { return this.currentQuestion; }
            set
            {
                this.currentQuestion = value;
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

        public ICommand ChoosableQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentQuestion != null && !ConfirmationWindow.ConfirmOverride())
                    {
                        return;
                    }

                    this.currentQuestion = new QuestionDTO
                    {
                        Type = QuestionTypesEnum.Choosable,
                        Answers = new ObservableCollection<AnswerDTO>
                        {
                            new AnswerDTO(),
                            new AnswerDTO(),
                            new AnswerDTO()
                        }
                    };

                    UpdateBindings();
                }); 
            }
        }

        public ICommand OpenQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentQuestion != null && !ConfirmationWindow.ConfirmOverride())
                    {
                        var result = MessageBox.Show("Вече създавате/променяте въпрос и създаването на друг ще отмени промените до момента. Сигурни ли сте?", "Предупреждение", MessageBoxButton.YesNo);

                        if (result != MessageBoxResult.Yes)
                        {
                            return;
                        }
                    }

                    this.currentQuestion = new QuestionDTO
                    {
                        Type = QuestionTypesEnum.Open,
                        Answers = new ObservableCollection<AnswerDTO> { new AnswerDTO() }
                    };

                    UpdateBindings();
                });
            }
        }

        public ICommand SaveQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentQuestion.ThemeId == 0 
                    || string.IsNullOrWhiteSpace(this.CurrentQuestion.Content) 
                    || this.CurrentQuestion.Answers.Any(a => string.IsNullOrWhiteSpace(a.Content)))
                    {
                        MessageBox.Show("Моля попълнете всички полета!");
                        return;
                    }

                    if (this.CurrentQuestion.Id == 0)
                    {
                        this.CurrentQuestion.Id = this.service.AddQuestion(this.CurrentQuestion, Authenticator.CurrentUser.Id);
                        this.UserQuestions.Add(this.CurrentQuestion);
                    }
                    else
                    {
                        this.service.UpdateQuestion(this.CurrentQuestion);
                    }

                    this.CurrentQuestion = null;
                    UpdateBindings();
                });
            }
        }

        public ICommand EditQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.CurrentQuestion != null && !ConfirmationWindow.ConfirmOverride())
                    {
                        return;
                    }

                    this.CurrentQuestion = (param as QuestionDTO);
                    UpdateBindings();
                });
            }
        }

        public ICommand DeleteQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!ConfirmationWindow.ConfirmDelete())
                    {
                       return;
                    }

                    if (this.CurrentQuestion == param)
                    {
                        this.CurrentQuestion = null;
                    }

                    this.UserQuestions.Remove((param as QuestionDTO));
                    this.service.DeleteQuestion((param as QuestionDTO).Id);

                    UpdateBindings();
                });
            }
        }

        public ICommand AddAnswerCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.CurrentQuestion.Answers.Add(new AnswerDTO());

                    UpdateBindings();
                });
            }
        }

        public ICommand CancelCurrentQuestion
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (!ConfirmationWindow.ConfirmCancel())
                    {
                        return;
                    }

                    this.CurrentQuestion = null;

                    UpdateBindings();
                });
            }
        }

        public ICommand RemoveAnswerCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.CurrentQuestion.Answers.Remove(param as AnswerDTO);

                    UpdateBindings();
                });
            }
        }

        public bool ShowQuestionForm
        {
            get
            {
                return this.currentQuestion != null;
            }
        }

        public bool ShowAnswersButtons
        {
            get
            {
                return this.CurrentQuestion != null && this.CurrentQuestion.Type == QuestionTypesEnum.Choosable;
            }
        }

        private void UpdateBindings()
        {
            RaisePropertyChanged(nameof(CurrentQuestion));
            RaisePropertyChanged(nameof(ShowQuestionForm));
            RaisePropertyChanged(nameof(ShowAnswersButtons));
            RaisePropertyChanged(nameof(UserQuestions));
        }
    }
}
