using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Subjects
    {
        public Subjects()
        {
            Themes = new HashSet<Themes>();
            Users = new HashSet<Users>();
            UsersSubjects = new HashSet<UsersSubjects>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Themes> Themes { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<UsersSubjects> UsersSubjects { get; set; }
    }
}
