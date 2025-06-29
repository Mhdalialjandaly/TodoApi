namespace Models
{
    public class CategoryWithTodosDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public IEnumerable<TodoItemDto> TodoItems { get; set; }
    }
}
