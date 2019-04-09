using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.Models;
using TeacherHelper.Services.Utils;

namespace TeacherHelper.Services.Services.Admin
{
    public class UsersService
    {
        private TeacherHelperContext dbContext;

        public UsersService()
        {
            this.dbContext = new TeacherHelperContext();
        }

        public List<UserDTO> LoadAllUsers()
        {
            var users = (from user in this.dbContext.Users
                         select new UserDTO
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Email = user.Email,
                             Username = user.Username,
                             IsAdmin = Convert.ToBoolean(user.IsAdmin),
                             IsActive = Convert.ToBoolean(user.IsActive),
                             Subjects = new ObservableCollection<SubjectDTO>(user.UsersSubjects
                             .Select(us => new SubjectDTO
                             {
                                 Id = us.SubjectId,
                                 Name = us.Subject.Name
                             })),
                             Subject = new SubjectDTO
                             {
                                 Id = user.CurrentSubject.Id,
                                 Name = user.CurrentSubject.Name
                             },
                             ProfilePicture = user.ProfilePicture
                         });

            return users.ToList();
        }

        public List<SubjectDTO> LoadSubjects()
        {
            return this.dbContext.Subjects.Select(s => new SubjectDTO
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();
        }

        public void DeleteUser(int id)
        {
            var user = this.dbContext.Users.Find(id);

            if (user == null)
            {
                return;
            }

            user.IsActive = 0;
            this.dbContext.SaveChanges();
        }

        public void RestoreUser(int id)
        {
            var user = this.dbContext.Users.Find(id);

            if (user == null)
            {
                return;
            }

            user.IsActive = 1;
            this.dbContext.SaveChanges();
        }

        public void RegisterUser(UserDTO currentUser)
        {
            var user = new Users
            {
                Name = currentUser.Name,
                Username = currentUser.Username,
                Email = currentUser.Email,
                Password = PasswordHasher.HashPassword(currentUser.Password),
                IsAdmin = currentUser.IsAdmin ? (byte)1 : (byte)0,
                CurrentSubjectId = currentUser.Subject.Id
            };

            user.UsersSubjects = currentUser.Subjects.Select(s => new UsersSubjects
            {
                SubjectId = s.Id,
                User = user
            }).ToArray();

            this.dbContext.UsersSubjects.AddRange(user.UsersSubjects);
            this.dbContext.Users.Add(user);

            this.dbContext.SaveChanges();
        }

        public void UpdateUser(UserDTO currentUser)
        {
            var user = dbContext.Users
                .Include(u => u.UsersSubjects)
                .ThenInclude(us => us.Subject)
                .FirstOrDefault(u => u.Id == currentUser.Id);

            if (user == null)
            {
                return;
            }
            
            user.Name = currentUser.Name;
            user.Username = currentUser.Username;
            user.IsAdmin = currentUser.IsAdmin ? (byte)1 : (byte)0;
            user.Email = currentUser.Email;
            user.CurrentSubjectId = currentUser.Subject.Id;

            if (!string.IsNullOrWhiteSpace(currentUser.Password))
            {
                user.Password = PasswordHasher.HashPassword(currentUser.Password);
            }

            foreach (var subj in user.UsersSubjects.ToList())
            {
                if (!currentUser.Subjects.Any(s => s.Id == subj.SubjectId))
                {
                    this.dbContext.UsersSubjects.Remove(subj);
                }
            }

            foreach (var subj in currentUser.Subjects)
            {
                if (!user.UsersSubjects.Any(us => us.SubjectId == subj.Id))
                {
                    this.dbContext
                        .UsersSubjects
                        .Add(new UsersSubjects
                        {
                            User = user,
                            SubjectId = subj.Id
                        });
                }
            }

            dbContext.SaveChanges();
        }
    }
}
