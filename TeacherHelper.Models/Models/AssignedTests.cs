using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class AssignedTests
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? TestId { get; set; }
        public string Class { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Tests Test { get; set; }
        public virtual Users User { get; set; }
    }
}
