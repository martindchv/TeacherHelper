using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherHelper.Models.DTOs.Common;

namespace TeacherHelper.Models.DTOs.Tests
{
    public class TestDTO
    {
        public int Id { get; set; }
        public ThemeDTO Theme { get; set; }
        public UserDTO Author { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
