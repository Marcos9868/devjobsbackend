using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DevJobsBackend.Dtos;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;

namespace DevJobsBackend.IoC.ProfileMapping
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<ResponseBase<User>, UserDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Data.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Data.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Data.Email))
                .ForMember(dest => dest.HashPassword, opt => opt.MapFrom(src => src.Data.HashPassword))
                .ForMember(dest => dest.TypeUser, opt => opt.MapFrom(src => src.Data.TypeUser));

        }
    }
}