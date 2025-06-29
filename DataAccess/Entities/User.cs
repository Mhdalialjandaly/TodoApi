using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class User : IdentityUser
    {
        public User() { TodoItems = new HashSet<TodoItem>(); }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public virtual ICollection<TodoItem> TodoItems { get; set; }
    }
}
