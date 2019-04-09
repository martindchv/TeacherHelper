using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.DTOs.QuestionsPage;
using TeacherHelper.Models.Enums;
using TeacherHelper.Models.Models;
using TeacherHelper.Services.Utils;

namespace TeacherHelper.Services.Services
{
    public class QuestionsService
    {
        private TeacherHelperContext db;

        public QuestionsService()
        {
            db = new TeacherHelperContext();
        }

        public List<QuestionDTO> GetUserQuestions(int id, int subjectId)
        {
            var questions = (from question in db.Questions.Include(q => q.Answers)
                             join theme in db.Themes on question.ThemeId equals theme.Id
                             where question.UserId == id && theme.SubjectId == subjectId
                             select new QuestionDTO
                             {
                                 Id = question.Id,
                                 Content = question.Content,
                                 IsPublic = Convert.ToBoolean(question.IsPublic),
                                 Type = (QuestionTypesEnum)Enum.Parse(typeof(QuestionTypesEnum), question.Type),
                                 ThemeId = theme.Id,
                                 Answers = new ObservableCollection<AnswerDTO>(question.Answers.Select(a => new AnswerDTO
                                 {
                                     Id = a.Id,
                                     Content = a.Content
                                 })),
                                 RightAnswer = question.Answers.Select(a => new AnswerDTO
                                 {
                                     Id = a.Id,
                                     Content = a.Content
                                 }).FirstOrDefault(a => a.Id == question.RightAnswerId)
                             });

            return questions.ToList();
        }

        public List<ThemeDTO> LoadThemes(int subjectId)
        {
            // In order to load all themes, where clause should be after ToList().
            var result = (from theme in db.Themes
                          where theme.SubjectId == subjectId
                          select theme).ToList().Where(t => t.ParentTheme == null).ToList();

            return ThemeTreeTraverser.FlattenThemesTree(result);
        }

        public int AddQuestion(QuestionDTO currentQuestion, int userId)
        {
            Questions questionToAdd = new Questions
            {
                Content = currentQuestion.Content,
                ThemeId = currentQuestion.ThemeId,
                UserId = userId,
                Type = currentQuestion.Type.ToString()
            };

            foreach (var answer in currentQuestion.Answers)
            {
                var answerToAdd = new Answers
                {
                    Content = answer.Content,
                    Question = questionToAdd
                };

                db.Answers.Add(answerToAdd);
            }

            db.Questions.Add(questionToAdd);

            db.SaveChanges();

            return questionToAdd.Id;
        }

        public void UpdateQuestion(QuestionDTO currentQuestion)
        {
            var question = db.Questions.Include(q => q.Answers).FirstOrDefault(q => q.Id == currentQuestion.Id);

            if (question == null)
            {
                return;
            }

            question.Content = currentQuestion.Content;
            question.IsPublic = (byte)(currentQuestion.IsPublic ? 1 : 0);
            question.ThemeId = currentQuestion.ThemeId;

            foreach (var ans in question.Answers)
            {
                if (!currentQuestion.Answers.Any(a => a.Id == ans.Id))
                {
                    db.Answers.Remove(ans);
                }
            }

            foreach (var ans in currentQuestion.Answers)
            {
                var answer = db.Answers.FirstOrDefault(a => a.Id == ans.Id);

                if (answer == null)
                {
                    db.Answers.Add(new Answers
                    {
                        Content = ans.Content,
                        Question = question
                    });
                }
                else if (answer.Content != ans.Content)
                {
                    answer.Content = ans.Content;
                }
            }

            db.SaveChanges();
        }

        public void DeleteQuestion(int id)
        {
            db.Answers.RemoveRange(db.Answers.Where(a => a.QuestionId == id));
            db.Questions.Remove(db.Questions.Find(id));

            db.SaveChanges();
        }
    }
}
