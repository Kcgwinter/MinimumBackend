using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //User Mappings
            CreateMap<UserRegisterDto, User>();
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<UserLogoutDto, User>();
        }
    }
}