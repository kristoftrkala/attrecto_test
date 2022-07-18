using Attrecto.Data;
using Microsoft.EntityFrameworkCore;

namespace Attrecto.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AttrectoTestDbContext _context;

        public TodoRepository(AttrectoTestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        public async Task<Todo> GetTodoByIdAsync(int todoId)
        {
            return await _context.Todos.FirstOrDefaultAsync(u => u.IdTodo == todoId);
        }

        public async Task<IEnumerable<Todo>> GetTodosForUserAsync(int userId)
        {
            return await _context.Todos.Where(t => t.FkUser == userId).ToListAsync();
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            if (todo == null)
            {
                throw new ArgumentException("Cannot be null");
            }
            todo.CreationDate = DateTime.Now;
            var result = await _context.Todos.AddAsync(todo);
            return result.Entity;
        }

        public async Task UpdateTodoAsync(Todo todo)
        {
            _context.Entry(todo).State = EntityState.Modified;
            await SaveChangesAsync();
        }

        public void DeleteTodo(int todoId)
        {
            var todo = _context.Todos.FirstOrDefault(x => x.IdTodo == todoId);
            if (todo == null)
                throw new ArgumentException("Todo not found");
            else
            {
                _context.Todos.Remove(todo);
                _context.SaveChanges();
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
