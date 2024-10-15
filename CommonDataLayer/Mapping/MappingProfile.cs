using AutoMapper;
using CommonDataLayer.DTOs;
using CommonDataLayer.Entities;
using CommonDataLayer.Model.RequestModels;

namespace CommonDataLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define mappings between DTOs and entities
            CreateMap<UserDto, Users>();
            CreateMap<Users, UserDto>();

            CreateMap<UserRegistrationRequestModel, Users>();

            // New mappings for additional tables
            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleDto>();


            // Add additional mappings as needed
        }
    }
}
