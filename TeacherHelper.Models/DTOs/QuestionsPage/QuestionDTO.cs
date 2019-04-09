using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeacherHelper.Models.Enums;

namespace TeacherHelper.Models.DTOs.QuestionsPage
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ObservableCollection<AnswerDTO> Answers { get; set; }
        public int ThemeId { get; set; }
        public QuestionTypesEnum Type { get; set; }
        public bool IsPublic { get; set; }
        public AnswerDTO RightAnswer { get; set; }
    }
}
