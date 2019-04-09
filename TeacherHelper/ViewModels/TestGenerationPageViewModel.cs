using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using TeacherHelper.Helpers;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.DTOs.TestGeneration;
using TeacherHelper.Models.Enums;
using TeacherHelper.Services.Services;
using MessageBox = System.Windows.Forms.MessageBox;
using Word = Microsoft.Office.Interop.Word;

namespace TeacherHelper.ViewModels
{
    public class TestGenerationPageViewModel : INotifyPropertyChanged
    {
        private TestGenerationService service;
        private ObservableCollection<ThemeDTO> allThemes;
        private ThemeDTO testTheme;
        private ObservableCollection<ThemeDTO> questionsThemes;
        private ThemeDTO questionsTheme;
        private List<QuestionDTO> allQuestions;
        private QuestionTypesEnum? questionsTypes;
        private QuestionDTO matchedQuestion;
        private QuestionDTO newQuestion;
        //private List<QuestionDTO> newQuestions;

        private FlowDocument testDocument;

        private bool showTestQuestions;
        private bool showAddExistingQuestion;
        private bool showAddNewQuestion;
        private bool showFinalizeForm;

        public TestGenerationPageViewModel()
        {
            this.service = new TestGenerationService();
            this.AllThemes = new ObservableCollection<ThemeDTO>(this.service.GetThemesBySubject(Authenticator.CurrentUser.Subject.Id));
            this.allQuestions = this.service.GetQuestionsBySubject(Authenticator.CurrentUser.Subject.Id);

            this.ShowTestQuestions = true;
            this.NewQuestion = new QuestionDTO();
        }

        public TestGenerationPageViewModel(int testId)
            : this()
        {
            var test = this.service.LoadTest(testId);
            this.TestTheme = this.allThemes.First(t => t.Id == test.Theme.Id);

            this.Test = test;

            for (int i = 0; i < this.Test.Questions.Count; i++)
            {
                VisualizeQuestion(this.Test.Questions[i], i);
            }
        }

        public TestDTO Test { get; set; }

        public string GetBackgroundColor
        {
            get
            {
                if (this.ShowAddExistingQuestion)
                {
                    return "#C58F6C";
                }
                else if (this.ShowTestQuestions)
                {
                    return "#D2AF99";
                }

                return "#A7705C";
            }
        }

        public QuestionDTO NewQuestion
        {
            get { return this.newQuestion; }
            set
            {
                this.newQuestion = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<ThemeDTO> AllThemes
        {
            get { return this.allThemes; }
            set
            {
                this.allThemes = value;
                this.RaisePropertyChanged();
            }
        }

        public object QuestionsType
        {
            get { return this.questionsTypes; }
            set
            {
                this.questionsTypes = (QuestionTypesEnum)Enum.Parse(typeof(QuestionTypesEnum), (value as ComboBoxItem).Name);
                this.RaisePropertyChanged();
            }
        }

        public ThemeDTO TestTheme
        {
            get { return this.testTheme; }
            set
            {
                this.testTheme = value;

                var themes = new List<ThemeDTO>();
                themes.Add(value);
                themes.AddRange(this.AllThemes
                    .Where(t => t.ParentThemesIds.Contains(value.Id)));
                this.QuestionsThemes = new ObservableCollection<ThemeDTO>(themes);

                if (this.Test != null && this.Test.Questions.Count > 0)
                {
                    this.allQuestions.AddRange(this.Test.Questions);
                }

                this.Test = new TestDTO();
                this.TestDocument = new FlowDocument();
                this.TestDocument.Background = Brushes.White;
                var para = new Paragraph(new Run(value.Name));
                para.TextAlignment = TextAlignment.Center;
                this.TestDocument.Blocks.Add(para);

                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<ThemeDTO> QuestionsThemes
        {
            get { return this.questionsThemes; }
            set
            {
                this.questionsThemes = value;
                this.RaisePropertyChanged();
            }
        }

        public ThemeDTO QuestionsTheme
        {
            get { return this.questionsTheme; }
            set
            {
                this.questionsTheme = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand FindQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.testTheme == null || this.questionsTypes == null)
                    {
                        return;
                    }

                    var filtred = this.allQuestions
                            .Where(q => q.QuestionType == this.questionsTypes
                                        && (q.ThemeId == this.questionsTheme.Id
                                        || this.AllThemes.First(t => t.Id == q.ThemeId)
                                                .ParentThemesIds.Contains(this.questionsTheme.Id))).ToArray();

                    if (filtred.Length == 0)
                    {
                        MessageBox.Show("Няма намерени въпроси.");
                        return;
                    }

                    Random rnd = new Random();

                    var index = rnd.Next(0, filtred.Length);

                    this.MatchedQuestion = filtred[index];
                });
            }
        }

        public QuestionDTO MatchedQuestion
        {
            get { return this.matchedQuestion; }
            set
            {
                this.matchedQuestion = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand AddQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    if (this.MatchedQuestion == null)
                    {
                        return;
                    }

                    this.Test.Questions.Add(this.MatchedQuestion);
                    this.allQuestions.Remove(this.MatchedQuestion);

                    VisualizeQuestion(this.MatchedQuestion, this.Test.Questions.Count);

                    this.MatchedQuestion = null;
                });
            }
        }

        public bool ShowTestQuestions
        {
            get { return this.showTestQuestions; }
            set
            {
                this.showTestQuestions = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.GetBackgroundColor));
            }
        }

        public bool ShowAddExistingQuestion
        {
            get { return this.showAddExistingQuestion; }
            set
            {
                this.showAddExistingQuestion = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.GetBackgroundColor));
            }
        }
        public bool ShowAddNewQuestion
        {
            get { return this.showAddNewQuestion; }
            set
            {
                this.showAddNewQuestion = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(this.GetBackgroundColor));
            }
        }

        public ICommand DisplayTestQuestionsCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.ShowAddExistingQuestion = false;
                    this.ShowAddNewQuestion = false;
                    this.ShowTestQuestions = true;
                });
            }
        }

        public ICommand DisplayAddExistingQuestionCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.ShowAddExistingQuestion = true;
                    this.ShowAddNewQuestion = false;
                    this.ShowTestQuestions = false;
                });
            }
        }

        public ICommand FinalizeTest
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.ShowFinalizeForm = true;
                });
            }
        }

        public ICommand SaveTestAsWord
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    this.Test.Theme = this.TestTheme;
                    this.service.CreateTest(this.Test, Authenticator.CurrentUser.Id);

                    var saveDialog = new SaveFileDialog();
                    saveDialog.FileName = this.TestTheme.Name + " - " + this.Test.Class;
                    saveDialog.DefaultExt = ".docx";
                    saveDialog.Filter = "Word documents (.docx)|*.docx";
                    var result = saveDialog.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        var filename = saveDialog.FileName;

                        Word._Application wordApp = new Word.Application();
                        wordApp.Visible = false;

                        Word.Document doc = wordApp.Documents.Add();

                        var titleParag = doc.Paragraphs.Add();
                        var titleRange = titleParag.Range;
                        titleRange.InsertBefore(this.Test.Theme.Name);
                        titleRange.Bold = 1;
                        titleParag.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                        var nameParagraph = doc.Paragraphs.Add();
                        var nameRange = nameParagraph.Range;
                        nameRange.InsertAfter("Име ......................................................................................................... Клас ");
                        if (string.IsNullOrWhiteSpace(this.Test.Class))
                        {
                            nameRange.InsertAfter("...........");
                        }
                        else
                        {
                            nameRange.InsertAfter(this.Test.Class);
                        }

                        nameRange.Bold = 1;

                        for (int i = 0; i < this.Test.Questions.Count; i++)
                        {
                            var para = doc.Paragraphs.Add();
                            var range = para.Range;

                            var questionContent = (i + 1).ToString() + ". " + this.Test.Questions[i].Content + Environment.NewLine;

                            range.Bold = 0;
                            range.InsertAfter(questionContent);
                            doc.Range(range.Start, range.Start + questionContent.Length).Bold = 1;

                            if (this.Test.Questions[i].QuestionType == QuestionTypesEnum.Choosable)
                            {
                                for (int j = 0; j < this.Test.Questions[i].Answers.Count; j++)
                                {
                                    var letter = (char)('A' + j);
                                    range.InsertAfter(letter.ToString() + ") " + this.Test.Questions[i].Answers[j].Content + Environment.NewLine);
                                }
                            }
                            else
                            {
                                range.InsertAfter(new string('.', this.Test.Questions.First().Content.Length * 4));
                            }
                        }

                        doc.SaveAs2(filename);
                        wordApp.Quit();
                    }
                });
            }
        }

        public ICommand BackToPrevPageCommand
        {
            get
            {
                return new RelayCommand((param) =>
                {
                    App.Current.Navigation.GoBack();
                });
            }
        }

        public FlowDocument TestDocument
        {
            get { return this.testDocument; }
            set
            {
                this.testDocument = value;
                this.RaisePropertyChanged();
            }
        }

        public bool ShowFinalizeForm
        {
            get { return this.showFinalizeForm; }
            set
            {
                this.showFinalizeForm = value;
                this.RaisePropertyChanged();
            }
        }

        private void VisualizeQuestion(QuestionDTO question, int position)
        {
            var paragraph = new Paragraph(new Bold(new Run((position + 1).ToString() + ". " + question.Content)));
            paragraph.ContentEnd.InsertLineBreak();

            if (question.QuestionType == QuestionTypesEnum.Choosable)
            {
                for (int i = 0; i < question.Answers.Count; i++)
                {
                    char letter = (char)('A' + i);
                    paragraph.ContentEnd.InsertTextInRun(letter.ToString() + ") " + question.Answers[i].Content);
                    paragraph.ContentEnd.InsertLineBreak();
                }
            }
            else
            {
                paragraph.ContentEnd.InsertTextInRun(new string('.', question.Answers.First().Content.Length * 4));
            }

            paragraph.ContentEnd.InsertParagraphBreak();
            this.TestDocument.Blocks.Add(paragraph);
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
