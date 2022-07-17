using Attrecto.Data;
using Attrecto.Dtos.User;
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
        private readonly IClaimsHelper _claimsHelper;

        public UserController(IUserRepository userRepository, IMapper mapper, IClaimsHelper claimsHelper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _claimsHelper = claimsHelper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<GetUserDto>> GetAllUser()
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);            
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
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
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
            mappedUser.Password = BCrypt.Net.BCrypt.HashPassword(mappedUser.Password);
            await _userRepository.CreateUserAsync(mappedUser);
            await _userRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<GetUserDto> AddUserFromAdmin(AddUserDto addUserDto)
        {
            if (_userRepository.CheckExistingEmail(addUserDto.Email))
            {
                throw new ArgumentException("This email is already in use.");
            }
            var mappedUser = _mapper.Map<User>(addUserDto);
            mappedUser.Password = BCrypt.Net.BCrypt.HashPassword(PasswordGenerator.Generate());
            var result = await _userRepository.CreateUserAsync(mappedUser);
            await _userRepository.SaveChangesAsync();
            return _mapper.Map<GetUserDto>(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            int claimId = _claimsHelper.GetIdFromClaim(HttpContext);
            if (!role.Equals("Admin") && claimId != updateUserDto.IdUser)
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }

            var current = await _userRepository.GetUserByIdAsync(id);
            if (current == null)
            {
                throw new ArgumentException("User not found");
            }

            current.Name = updateUserDto.Name;
            current.Email = updateUserDto.Email;
            await _userRepository.UpdateUserAsync(current);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string role = _claimsHelper.GetRoleFromClaim(HttpContext);
            if (!role.Equals("Admin"))
            {
                throw new UnauthorizedAccessException("Only admins can do this");
            }
            _userRepository.DeleteUser(id);
            return NoContent();
        }
    }
}
