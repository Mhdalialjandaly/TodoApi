using Core.Enums;

namespace DataAccess.Services.Response
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<string> Role { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public int TodoItemsCount { get; set; } 
        public int CompletedTodosCount { get; set; } 
    }
}
