using Attrecto.Data;
using Attrecto.Dtos;
using AutoMapper;

namespace Attrecto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<AddUserDto, User>();
            CreateMap<User, GetUserDto>();
        }
    }
}
