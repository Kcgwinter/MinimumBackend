using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            CreateMap<User, UserResponseDto>();

        }
    }
}