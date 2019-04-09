using System;
using System.Globalization;
using System.Windows.Data;
using TeacherHelper.Models.DTOs.QuestionsPage;

namespace TeacherHelper.Converters
{
    public class QuestionToAnswersCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var question = (value as QuestionDTO);

            if (question == null)
            {
                return null;
            }

            return question.Answers.Count >= 3 ? 3 : question.Answers.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
