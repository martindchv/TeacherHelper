using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.DTOs.Tests;
using TeacherHelper.Models.Enums;
using TeacherHelper.Models.Models;

namespace TeacherHelper.Services.Services
{
    public class TestService
    {
        private TeacherHelperContext db;
        public TestService()
        {
            db = new TeacherHelperContext();
        }
        public List<TestDTO> GetUserTests(int id)
        {
            var tests = (from test in db.Tests
                         join author in db.Users on test.AuthorId equals author.Id
                         join theme in db.Themes on test.ThemeId equals theme.Id
                         where test.AuthorId == id
                         select new TestDTO
                         {
                             Id = test.Id,
                             Theme = new ThemeDTO
                             {
                                 Id = theme.Id,
                                 Name = theme.Name
                             },
                             Author = new Models.DTOs.Tests.UserDTO
                             {
                                 Id = author.Id,
                                 Name = author.Name,
                                 Username = author.Username,
                                 ProfilePicture = author.ProfilePicture
                             },
                             DateCreated = test.DateCreated
                        });
            return tests.ToList();
        }
    }
}
