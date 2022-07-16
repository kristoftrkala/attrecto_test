using Attrecto.Data;
using Attrecto.Dtos;
using Attrecto.Repositories;
using AutoMapper;
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

        [HttpPut]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
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
    }
}
