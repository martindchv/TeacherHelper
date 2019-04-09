using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Questions
    {
        public Questions()
        {
            Answers = new HashSet<Answers>();
            TestsQuestions = new HashSet<TestsQuestions>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int ThemeId { get; set; }
        public byte IsPublic { get; set; }
        public int UserId { get; set; }
        public int? RightAnswerId { get; set; }

        public virtual Answers RightAnswer { get; set; }
        public virtual Themes Theme { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<Answers> Answers { get; set; }
        public virtual ICollection<TestsQuestions> TestsQuestions { get; set; }
    }
}
