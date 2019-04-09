using TeacherHelper.Models.DTOs.Common;

namespace TeacherHelper.Helpers
{
    public static class Authenticator
    {
        public static UserDTO CurrentUser { get; private set; }

        public static void UpdateUser(UserDTO user)
        {
            CurrentUser = user;
            NavBarHelper.UpdateUserData();
        }
    }
}
