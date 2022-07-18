namespace Attrecto.Dtos.Todo
{
    public class AddTodoByAdminDto
    {
        public int IdUser { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
