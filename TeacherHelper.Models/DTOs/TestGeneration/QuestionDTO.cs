using System.Collections.Generic;
using TeacherHelper.Models.Enums;

namespace TeacherHelper.Models.DTOs.TestGeneration
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int ThemeId { get; set; }
        public List<AnswerDTO> Answers { get; set; }
        public QuestionTypesEnum QuestionType { get; set; }
    }
}
