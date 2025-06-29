using Core.Enums;

namespace DataAccess.Entities
{
    public class TodoItem 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityLevel Priority { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Created { get ; set ; }
        public string CreatedBy { get ; set ; }
        public DateTime? LastModified { get ; set ; }
        public string LastModifiedBy { get ; set ; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime UpdatedAt { get; internal set; }
    }
}
