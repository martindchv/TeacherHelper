using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Answers
    {
        public Answers()
        {
            Questions = new HashSet<Questions>();
        }

        public int Id { get; set; }
        public string Content { get; set; }
        public int QuestionId { get; set; }

        public virtual Questions Question { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
    }
}
