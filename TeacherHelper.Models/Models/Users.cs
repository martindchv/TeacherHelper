using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Users
    {
        public Users()
        {
            AssignedTests = new HashSet<AssignedTests>();
            Questions = new HashSet<Questions>();
            Tests = new HashSet<Tests>();
            UsersSubjects = new HashSet<UsersSubjects>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte IsAdmin { get; set; }
        public byte IsActive { get; set; }
        public int? CurrentSubjectId { get; set; }
        public byte[] ProfilePicture { get; set; }

        public virtual Subjects CurrentSubject { get; set; }
        public virtual ICollection<AssignedTests> AssignedTests { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<Tests> Tests { get; set; }
        public virtual ICollection<UsersSubjects> UsersSubjects { get; set; }
    }
}
