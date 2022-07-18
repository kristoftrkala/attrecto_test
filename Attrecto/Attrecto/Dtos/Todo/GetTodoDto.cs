namespace Attrecto.Dtos.Todo
{
    public class GetTodoDto
    {
        public int IdTodo { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastModifyDate { get; set; }
    }
}
