using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class UsersSubjects
    {
        public int SubjectId { get; set; }
        public int UserId { get; set; }

        public virtual Subjects Subject { get; set; }
        public virtual Users User { get; set; }
    }
}
