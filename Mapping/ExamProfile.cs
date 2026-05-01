using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class ExamProfile : Profile
    {
        public ExamProfile()
        {
            CreateMap<ExamCsvDto, Exam>()
                .ForMember(dest => dest.ExamId, opt => opt.Ignore());
        }
    }
}