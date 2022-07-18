namespace Attrecto.Dtos.Todo
{
    public class UpdateTodoDto
    {
        public int IdTodo { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
