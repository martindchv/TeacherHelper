using System.Collections.Generic;
using System.Linq;
using TeacherHelper.Models.DTOs.Common;
using TeacherHelper.Models.Models;

namespace TeacherHelper.Services.Utils
{
    public class ThemeTreeTraverser
    {
        public static List<ThemeDTO> FlattenThemesTree(IEnumerable<Themes> themes, int depth = 0, ThemeDTO parentTheme = null)
        {
            if (themes == null)
            {
                return null;
            }

            var result = new List<ThemeDTO>();

            foreach (var theme in themes)
            {
                var t = new ThemeDTO
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    TreeDepth = depth
                };

                if (parentTheme != null)
                {
                    t.ParentThemesIds.AddRange(parentTheme.ParentThemesIds);
                    t.ParentThemesIds.Add(parentTheme.Id);
                }

                result.Add(t);

                result.AddRange(FlattenThemesTree(theme.InverseParentTheme, depth + 1, t));
            }

            return result;
        }

        public static List<Themes> OrderThemes(List<Themes> themes)
        {
            var theme = themes.FirstOrDefault(t => t.PreviousTheme == null);

            if (theme == null)
            {
                return themes;
            }

            var orderedThemes = new List<Themes>();
            theme.InverseParentTheme = OrderThemes(theme.InverseParentTheme.ToList());
            orderedThemes.Add(theme);
            themes.Remove(theme);

            while (themes.Count != 0)
            {
                var nextTheme = themes.FirstOrDefault(t => t.PreviousTheme?.Id == theme.Id);

                if (nextTheme != null)
                {
                    nextTheme.PreviousTheme = theme;
                    nextTheme.InverseParentTheme = OrderThemes(nextTheme.InverseParentTheme.ToList());
                    orderedThemes.Add(nextTheme);
                    themes.Remove(nextTheme);
                    theme = nextTheme;
                }
                else
                {
                    break;
                }
            }

            return orderedThemes;
        }
    }
}
