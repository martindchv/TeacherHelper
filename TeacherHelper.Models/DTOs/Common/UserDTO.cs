using System.Collections.ObjectModel;

namespace TeacherHelper.Models.DTOs.Common
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public SubjectDTO Subject { get; set; }
        public ObservableCollection<SubjectDTO> Subjects { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
