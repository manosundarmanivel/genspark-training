
using AutoMapper;
using ElearnAPI.DTOs;
using ElearnAPI.Models;

namespace ElearnAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserDtoResponse>().ReverseMap();
            CreateMap<CreateUserDto, User>();
            CreateMap<CourseDto, Course>();
            CreateMap<Course, CourseDto>();
            CreateMap<UploadedFileDto, UploadedFile>();
            CreateMap<UploadedFileDto, UploadedFile>().ReverseMap();
            CreateMap<CourseDto, Course>().ReverseMap();
        }
    }
}
