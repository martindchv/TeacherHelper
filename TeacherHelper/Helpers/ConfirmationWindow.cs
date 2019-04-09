using System.Windows;

namespace TeacherHelper.Helpers
{
    public static class ConfirmationWindow
    {
        public static bool ConfirmDelete()
        {
            var result = MessageBox.Show("Сигурни ли сте, че искате да го изтриете?", "Предупреждение", MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }

        public static bool ConfirmCancel()
        {
            var result = MessageBox.Show("Сигурни ли сте, че искате да го отмените? Промените няма да бъдат запазени!", "Предупреждение", MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }

        public static bool ConfirmOverride()
        {
            var result = MessageBox.Show("Това действие ще отмени всички промени до момента! Сигурни ли сте, че искате да го извършите?", "Предупреждение", MessageBoxButton.YesNo);

            return result == MessageBoxResult.Yes;
        }
    }
}
