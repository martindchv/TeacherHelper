using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Tests
    {
        public Tests()
        {
            AssignedTests = new HashSet<AssignedTests>();
            TestsQuestions = new HashSet<TestsQuestions>();
        }

        public int Id { get; set; }
        public string Class { get; set; }
        public int ThemeId { get; set; }
        public int AuthorId { get; set; }
        public byte IsPublic { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Users Author { get; set; }
        public virtual Themes Theme { get; set; }
        public virtual ICollection<AssignedTests> AssignedTests { get; set; }
        public virtual ICollection<TestsQuestions> TestsQuestions { get; set; }
    }
}
