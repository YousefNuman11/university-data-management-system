using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<StudentCsvDto, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore());
        }
    }
}