using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class CourseInstructorProfile : Profile
    {
        public CourseInstructorProfile()
        {
            CreateMap<CourseInstructorCsvDto, CourseInstructor>();
        }
    }
}
