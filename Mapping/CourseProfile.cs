using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseCsvDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());
        }
    }
}