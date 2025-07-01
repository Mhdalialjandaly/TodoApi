namespace Models
{
    public class UserDto
    {
        public string FullName { get; set; }
        public virtual ICollection<TodoItemDto> TodoItems { get; set; }
    }
}