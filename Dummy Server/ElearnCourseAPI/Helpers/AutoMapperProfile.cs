
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
            CreateMap<CreateCourseDto, Course>();
            CreateMap<UploadedFileDto, UploadedFile>();
            CreateMap<UploadedFileDto, UploadedFile>().ReverseMap();
            CreateMap<CourseDto, Course>().ReverseMap();
            CreateMap<CreateCouponDto, CouponCode>();
            CreateMap<CouponCode, CouponResponseDto>();
        
             CreateMap<CreateTransactionDto, Transaction>();
        CreateMap<Transaction, TransactionDto>()
    .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course!.Title))
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.FullName));

        }
    }
}
