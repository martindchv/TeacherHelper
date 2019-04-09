using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Answer
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int QuestionId { get; set; }

        public virtual Questions Question { get; set; }
    }
}
