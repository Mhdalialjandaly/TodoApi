using Core.Enums;

namespace TodoList.Api.RequestModel.Todo
{
    public class TodoRequestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityLevel Priority { get; set; }
        public int CategoryId { get; set; }
    }
}
