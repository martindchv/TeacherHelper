using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherHelper.Models.DTOs.Admin.Subjects
{
    public class SubjectDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ThemeDTO> Themes { get; set; }
    }
}
