using System.Collections.Generic;
using System.Collections.ObjectModel;
using TeacherHelper.Models.DTOs.Common;

namespace TeacherHelper.Models.DTOs.TestGeneration
{
    public class TestDTO
    {
        public TestDTO()
        {
            this.Questions = new ObservableCollection<QuestionDTO>();
        }

        public int Id { get; set; }
        public string Class { get;
            set; }
        public ObservableCollection<QuestionDTO> Questions { get; set; }
        public ThemeDTO Theme { get; set; }
        public bool IsPublic { get; set; }
    }
}
