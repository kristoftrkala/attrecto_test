using Attrecto.Data;
using Attrecto.Dtos.Todo;
using Attrecto.Dtos.User;
using AutoMapper;

namespace Attrecto
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //user Dtos
            CreateMap<CreateUserDto, User>();
            CreateMap<AddUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, GetUserDto>();

            //todo Dtos
            CreateMap<Todo, GetTodoDto>();
            CreateMap<AddTodoByUserDto, Todo>();
            CreateMap<AddTodoByAdminDto, Todo>()
                .ForMember(dst => dst.FkUser, cfg => cfg.MapFrom(src => src.IdUser));
        }
    }
}
