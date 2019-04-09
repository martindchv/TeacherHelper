using System;
using System.Collections.Generic;

namespace TeacherHelper.Models.Models
{
    public partial class Themes
    {
        public Themes()
        {
            InverseParentTheme = new HashSet<Themes>();
            InversePreviousTheme = new HashSet<Themes>();
            Questions = new HashSet<Questions>();
            Tests = new HashSet<Tests>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public int? ParentThemeId { get; set; }
        public int? PreviousThemeId { get; set; }

        public virtual Themes ParentTheme { get; set; }
        public virtual Themes PreviousTheme { get; set; }
        public virtual Subjects Subject { get; set; }
        public virtual ICollection<Themes> InverseParentTheme { get; set; }
        public virtual ICollection<Themes> InversePreviousTheme { get; set; }
        public virtual ICollection<Questions> Questions { get; set; }
        public virtual ICollection<Tests> Tests { get; set; }
    }
}
