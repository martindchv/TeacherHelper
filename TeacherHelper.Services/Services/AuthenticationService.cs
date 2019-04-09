using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.Models;
using TeacherHelper.Services.Utils;

namespace TeacherHelper.Services.Services
{
    public class AuthenticationService
    {
        private TeacherHelperContext dbContext;

        public AuthenticationService()
        {
            dbContext = new TeacherHelperContext();
        }

        public UserDTO Login(string username, string password)
        {
            var result = dbContext.Users
                .Include(u => u.CurrentSubject)
                .Include(u => u.UsersSubjects)
                .ThenInclude(us => us.Subject)
                .FirstOrDefault(u => u.Username == username && Convert.ToBoolean(u.IsActive));

            if (result == null)
            {
                return null;
            }

            if (PasswordHasher.CheckPassword(result.Password, password))
            {
                return new UserDTO
                {
                    Id = result.Id,
                    Name = result.Name,
                    // TODO
                    Subject = result.CurrentSubject != null ? 
                                new SubjectDTO { Id = result.CurrentSubject.Id, Name = result.CurrentSubject.Name } 
                                : new SubjectDTO { Id = 1, Name = "Test" },
                    Username = result.Username,
                    Email = result.Email,
                    IsAdmin = Convert.ToBoolean(result.IsAdmin),
                    IsActive = Convert.ToBoolean(result.IsActive),
                    Subjects = new ObservableCollection<SubjectDTO>(result.UsersSubjects.Select(us => new SubjectDTO
                    {
                        Id = us.SubjectId,
                        Name = us.Subject.Name
                    })),
                    ProfilePicture = result.ProfilePicture
                };
            }

            return null;
        }

        public void Register(string username, string password, string name, string email, bool is_admin)
        {
            var user = new Users
            {
                Name = name,
                Username = username,
                Email = email,
                IsAdmin = is_admin ? (byte)1 : (byte)0,
                Password = PasswordHasher.HashPassword(password)
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}
