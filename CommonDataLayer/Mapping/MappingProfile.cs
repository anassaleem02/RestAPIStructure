using AutoMapper;
using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;

namespace CommonDataLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define mappings between DTOs and entities
            CreateMap<UserDto, Users>();
            CreateMap<Users, UserDto>();

            // New mappings for additional tables
            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleDto>();

            // Add additional mappings as needed
        }
    }
}
