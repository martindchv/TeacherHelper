using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TeacherHelper.Models.DTOs.Admin.Subjects;
using TeacherHelper.Models.Models;

namespace TeacherHelper.Services.Services.Admin
{
    public class SubjectsService
    {
        private TeacherHelperContext dbContext;

        public SubjectsService()
        {
            this.dbContext = new TeacherHelperContext();
        }

        public List<SubjectDTO> GetSubjects()
        {
            var themes = this.dbContext.Themes.ToList();

            var subjects = (from subject in this.dbContext.Subjects
                            select new SubjectDTO
                            {
                                Id = subject.Id,
                                Name = subject.Name,
                                Themes = new ObservableCollection<ThemeDTO>(themes
                                            .Where(t => t.SubjectId == subject.Id && t.ParentTheme == null)
                                            .Select(t => this.MapTheme(t, 0)))
                            }).ToList();

            subjects.ForEach(s => s.Themes = this.OrderThemes(s.Themes));

            return subjects;
        }

        public void UpdateSubject(SubjectDTO currentSubject)
        {
            var subject = this.dbContext.Subjects
                                .Include(s => s.Themes)
                                .First(s => s.Id == currentSubject.Id);

            if (subject == null)
            {
                return;
            }

            subject.Name = currentSubject.Name;

            var themes = this.FlattenTree(currentSubject.Themes);

            foreach (var theme in subject.Themes.ToList())
            {
                if (!themes.Any(t => t.Id == theme.Id))
                {
                    this.dbContext.Themes.Remove(theme);
                }
            }

            Dictionary<ThemeDTO, Themes> newThemesReferences = new Dictionary<ThemeDTO, Themes>();

            foreach (var theme in themes)
            {
                if (theme.Id != 0)
                {
                    var t = this.dbContext.Themes.Find(theme.Id);

                    t.Name = theme.Name;

                    if (theme.PreviousTheme == null && t.PreviousThemeId != null)
                    {
                        t.PreviousTheme = null;
                    }
                    else if (theme.PreviousTheme != null)
                    {
                        if (theme.PreviousTheme.Id != 0 && t.PreviousThemeId != theme.PreviousTheme.Id)
                        {
                            t.PreviousThemeId = theme.PreviousTheme.Id;
                        }
                        else if (theme.PreviousTheme.Id == 0)
                        {
                            t.PreviousTheme = newThemesReferences[theme.PreviousTheme];
                        }
                    }
                }
                else
                {
                    var th = new Themes
                    {
                        Name = theme.Name,
                        Subject = subject
                    };

                    this.dbContext.Themes.Add(th);

                    newThemesReferences[theme] = th;

                    if (theme.ParentTheme != null)
                    {
                        if (theme.ParentTheme.Id == 0)
                        {
                            th.ParentTheme = newThemesReferences[theme.ParentTheme];
                        }
                        else
                        {
                            th.ParentTheme = this.dbContext.Themes.Find(theme.ParentTheme.Id);
                        }
                    }

                    if (theme.PreviousTheme != null)
                    {
                        if (theme.PreviousTheme.Id == 0)
                        {
                            th.PreviousTheme = newThemesReferences[theme.PreviousTheme];
                        }
                        else
                        {
                            th.PreviousTheme = this.dbContext.Themes.Find(theme.PreviousTheme.Id);
                        }
                    }
                }
            }

            this.dbContext.SaveChanges();
        }

        private List<ThemeDTO> FlattenTree(ObservableCollection<ThemeDTO> themes)
        {
            var result = new List<ThemeDTO>();
            foreach (var theme in themes)
            {
                result.Add(theme);
                result.AddRange(this.FlattenTree(theme.ChildThemes));
            }

            return result;
        }

        public void CreateSubject(SubjectDTO currentSubject)
        {
            var subject = new Subjects
            {
                Name = currentSubject.Name
            };

            this.AddThemesToDatabase(subject, null, currentSubject.Themes);

            this.dbContext.Subjects.Add(subject);

            this.dbContext.SaveChanges();
        }

        private void AddThemesToDatabase(Subjects subject, Themes parentTheme, ObservableCollection<ThemeDTO> childThemes)
        {
            Themes lastTheme = null;
            foreach (var th in childThemes)
            {
                var theme = new Themes
                {
                    Name = th.Name,
                    Subject = subject,
                    ParentTheme = parentTheme,
                    PreviousTheme = lastTheme
                };

                this.dbContext.Themes.Add(theme);

                this.AddThemesToDatabase(subject, theme, th.ChildThemes);

                lastTheme = theme;
            }
        }

        public void DeleteSubject(SubjectDTO subjectDTO)
        {
            var subject = this.dbContext.Subjects.Find(subjectDTO.Id);

            if (subject == null)
            {
                return;
            }

            this.dbContext.Subjects.Remove(subject);

            this.dbContext.SaveChanges();
        }

        private ThemeDTO MapTheme(Themes t, int depth = 0)
        {
            if (t == null)
            {
                return null;
            }

            var result = new ThemeDTO
            {
                Id = t.Id,
                Name = t.Name,
                PreviousTheme = this.MapNullableTheme(t.PreviousTheme),
                ParentTheme = this.MapNullableTheme(t.ParentTheme),
                TreeDepth = depth,
                ChildThemes = new ObservableCollection<ThemeDTO>(t.InverseParentTheme
                                        .Select(th => this.MapTheme(th, depth + 1)))
            };

            return result;
        }

        private ObservableCollection<ThemeDTO> OrderThemes(ObservableCollection<ThemeDTO> themes)
        {
            var theme = themes.FirstOrDefault(t => t.PreviousTheme == null);

            if (theme == null)
            {
                return themes;
            }

            var orderedThemes = new List<ThemeDTO>();
            theme.ChildThemes = this.OrderThemes(theme.ChildThemes);
            orderedThemes.Add(theme);
            themes.Remove(theme);

            while (themes.Count != 0)
            {
                var nextTheme = themes.FirstOrDefault(t => t.PreviousTheme?.Id == theme.Id);

                if (nextTheme != null)
                {
                    nextTheme.PreviousTheme = theme;
                    theme.NextTheme = nextTheme;
                    nextTheme.ChildThemes = this.OrderThemes(nextTheme.ChildThemes);
                    orderedThemes.Add(nextTheme);
                    themes.Remove(nextTheme);
                    theme = nextTheme;
                }
                else
                {
                    theme.NextTheme = null;
                    break;
                }
            }

            foreach (var t in themes)
            {
                t.TreeDepth = this.CountParentElements(t);
            }

            return new ObservableCollection<ThemeDTO>(orderedThemes);
        }

        private int CountParentElements(ThemeDTO theme)
        {
            if (theme.ParentTheme == null)
            {
                return 0;
            }

            return 1 + this.CountParentElements(theme);
        }

        private ThemeDTO MapNullableTheme(Themes previousTheme)
        {
            if (previousTheme == null)
            {
                return null;
            }

            return new ThemeDTO
            {
                Id = previousTheme.Id
            };
        }
    }
}
