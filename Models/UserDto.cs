using Core.Enums;

namespace Models
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public virtual ICollection<TodoItemDto> TodoItems { get; set; }
    }
}