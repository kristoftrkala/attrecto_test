using Attrecto.Data;

namespace Attrecto.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetAllTodosAsync();
        Task<Todo> GetTodoByIdAsync(int todoId);
        Task<IEnumerable<Todo>> GetTodosForUserAsync(int userId);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task UpdateTodoAsync(Todo todo);
        void DeleteTodo(int todoId);
        Task SaveChangesAsync();
    }
}
