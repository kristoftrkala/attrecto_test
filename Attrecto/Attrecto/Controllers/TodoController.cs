using Attrecto.Data;
using Attrecto.Dtos.Todo;
using Attrecto.EmailService;
using Attrecto.IdentityServer;
using Attrecto.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attrecto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly IMapper _mapper;
        private readonly IClaimsHelper _claimsHelper;
        private readonly IEmailService _emailService;

        public TodoController(ITodoRepository todoRepository, IMapper mapper, IClaimsHelper claimsHelper, IEmailService emailService)
        {
            _todoRepository = todoRepository;
            _mapper = mapper;
            _claimsHelper = claimsHelper;
            _emailService = emailService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<GetTodoDto>> GetAllTodos()
        {
            var todos = await _todoRepository.GetAllTodosAsync();
            return _mapper.Map<IEnumerable<GetTodoDto>>(todos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<GetTodoDto> GetTodoById(int id)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id);
            return _mapper.Map<GetTodoDto>(todo);
        }

        [HttpGet]
        [Authorize]
        [Route("user/{id}")]
        public async Task<IEnumerable<GetTodoDto>> GetTodosForUser(int id)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
            if (!role.Equals("Admin") && id != claimId)
            {
                throw new UnauthorizedAccessException("You can't do this");
            }
            var todos = await _todoRepository.GetTodosForUserAsync(id);
            return _mapper.Map<IEnumerable<GetTodoDto>>(todos);
        }

        [HttpPost]
        [Authorize]
        public async Task<GetTodoDto> CreateTodoByUser(AddTodoByUserDto todoDto)
        {
            var mappedTodo = _mapper.Map<Todo>(todoDto);
            mappedTodo.FkUser = _claimsHelper.GetIdFromClaim(HttpContext);
            var result = await _todoRepository.CreateTodoAsync(mappedTodo);
            await _todoRepository.SaveChangesAsync();
            return _mapper.Map<GetTodoDto>(result);
        }

        [HttpPost]
        [Authorize]
        [Route("addByAdmin")]
        public async Task<GetTodoDto> CreateTodoByAdmin(AddTodoByAdminDto todoDto)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            if (!role.Equals("Admin"))
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            var mappedTodo = _mapper.Map<Todo>(todoDto);
            var result = await _todoRepository.CreateTodoAsync(mappedTodo);
            await _emailService.SendEmailAsync(result.FkUserNavigation.Email, "New TODO", $"A new TODO has been assigned to you. Let's check it.");
            return _mapper.Map<GetTodoDto>(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto updateTodoDto)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
            var currentTodo = await _todoRepository.GetTodoByIdAsync(id);
            if (currentTodo == null)
            {
                throw new ArgumentException("Todo not found");
            }
            if (!role.Equals("Admin") && currentTodo.FkUser != claimId)
            {
                throw new UnauthorizedAccessException("You can't do this");
            }

            currentTodo.Name = updateTodoDto.Name;
            currentTodo.Description = updateTodoDto.Description;
            currentTodo.LastModifyDate = DateTime.Now;
            await _todoRepository.UpdateTodoAsync(currentTodo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            int userId = _claimsHelper.GetIdFromClaim(HttpContext);
            var todo = await _todoRepository.GetTodoByIdAsync(id);
            if (!role.Equals("Admin") && todo.FkUser != userId)
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            _todoRepository.DeleteTodo(id);
            return NoContent();
        }
    }
}
