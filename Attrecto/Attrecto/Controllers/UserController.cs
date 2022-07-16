using Attrecto.Data;
using Attrecto.Dtos;
using Attrecto.IdentityServer;
using Attrecto.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attrecto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<GetUserDto>> GetAllUser()
        {
            string role = ClaimsHelper.GetRoleFromClaim(HttpContext);            
            if(!role.Equals("Admin"))
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            var users = await _userRepository.GetAllUserAsync();
            return _mapper.Map<IEnumerable<GetUserDto>>(users);

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<GetUserDto> GetUserById(int id)
        {
            string role = ClaimsHelper.GetRoleFromClaim(HttpContext);
            int claimId = ClaimsHelper.GetIdFromClaim(HttpContext);
            if (!role.Equals("Admin") && claimId != id)
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<GetUserDto>(user);

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser(CreateUserDto createUserDto)
        {
            if (_userRepository.CheckExistingEmail(createUserDto.Email))
            {
                throw new ArgumentException("This email is already in use.");
            }
            var mappedUser = _mapper.Map<User>(createUserDto);
            await _userRepository.CreateUserAsync(mappedUser);
            await _userRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddUserFromAdmin(AddUserDto addUserDto)
        {
            if (_userRepository.CheckExistingEmail(addUserDto.Email))
            {
                throw new ArgumentException("This email is already in use.");
            }
            var mappedUser = _mapper.Map<User>(addUserDto);
            mappedUser.Password = PasswordGenerator.Generate();
            await _userRepository.CreateUserAsync(mappedUser);
            await _userRepository.SaveChangesAsync();
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string role = ClaimsHelper.GetRoleFromClaim(HttpContext);
            if (!role.Equals("Admin"))
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            _userRepository.DeleteUser(id);
            return Ok();
        }
    }
}
