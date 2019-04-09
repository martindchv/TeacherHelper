using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherHelper.Models.DTOs.Common
{
    public class ThemeDTO
    {
        public ThemeDTO()
        {
            this.ParentThemesIds = new List<int>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int TreeDepth { get; set; }
        public List<int> ParentThemesIds { get; set; }
    }
}
