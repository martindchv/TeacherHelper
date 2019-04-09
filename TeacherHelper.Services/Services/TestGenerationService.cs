using System.Collections.Generic;
using TeacherHelper.Models.Models;
using System.Linq;
using TeacherHelper.Services.Utils;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.DTOs.TestGeneration;
using System;
using TeacherHelper.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace TeacherHelper.Services.Services
{
    public class TestGenerationService
    {
        private TeacherHelperContext db;

        public TestGenerationService()
        {
            this.db = new TeacherHelperContext();
        }

        public List<ThemeDTO> GetThemesBySubject(int subjectId)
        {
            // In order to load all themes, where clause should be after ToList().
            var result = (from theme in db.Themes
                          where theme.SubjectId == subjectId
                          select theme).ToList().Where(t => t.ParentTheme == null)
                          .ToList();

            return ThemeTreeTraverser.FlattenThemesTree(ThemeTreeTraverser.OrderThemes(result));
        }

        public List<QuestionDTO> GetQuestionsBySubject(int subjectId)
        {
            var result = (from question in db.Questions
                          join theme in db.Themes on question.ThemeId equals theme.Id
                          where theme.SubjectId == subjectId
                          select new QuestionDTO
                          {
                              Id = question.Id,
                              Content = question.Content,
                              QuestionType = (QuestionTypesEnum)Enum.Parse(typeof(QuestionTypesEnum), question.Type),
                              ThemeId = question.ThemeId,
                              Answers = question.Answers.Select(a => new AnswerDTO
                              {
                                  Id = a.Id,
                                  Content = a.Content
                              }).ToList()
                          }).ToList();

            return result;
        }

        public void CreateTest(TestDTO test, int id)
        {
            var testToAdd = new Tests
            {
                Class = test.Class,
                DateCreated = DateTime.Now,
                AuthorId = id,
                IsPublic = (byte)(test.IsPublic ? 1 : 0),
                ThemeId = test.Theme.Id
            };

            for (int i = 0; i < test.Questions.Count; i++)
            {
                var tq = new TestsQuestions
                {
                    Test = testToAdd,
                    QuestionId = test.Questions[i].Id,
                    QuestionPlace = i
                };

                this.db.TestsQuestions.Add(tq);
            }

            this.db.Tests.Add(testToAdd);

            db.SaveChanges();
        }

        public TestDTO LoadTest(int testId)
        {
            var test = this.db.Tests
                .Include(t => t.Theme)
                .Include(t => t.TestsQuestions)
                .FirstOrDefault(t => t.Id == testId);

            var questions = this.db.Questions
                .Include(q => q.Answers)
                .Where(q => test.TestsQuestions.Any(tq => tq.QuestionId == q.Id))
                .ToList();

            if (test == null)
            {
                return null;
            }

            var testDTO = new TestDTO
            {
                Class = test.Class,
                Theme = new ThemeDTO
                {
                    Id = test.ThemeId,
                    Name = test.Theme.Name
                },
                Questions = new ObservableCollection<QuestionDTO>(questions.Select(q => new QuestionDTO
                {
                    Id = q.Id,
                    Content = q.Content,
                    QuestionType = (QuestionTypesEnum)Enum.Parse(typeof(QuestionTypesEnum), q.Type),
                    Answers = q.Answers.Select(a => new AnswerDTO
                    {
                        Id = a.Id,
                        Content = a.Content
                    }).ToList()
                }))
            };

            return testDTO;
        }
    }
}
