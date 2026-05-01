using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class InstructorProfile : Profile
    {
        public InstructorProfile()
        {
            CreateMap<InstructorCsvDto, Instructor>()
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore());
        }
    }
}