using AutoMapper;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<DepartmentCsvDto, Department>()
                .ForMember(dest => dest.DepartmentId, opt => opt.Ignore());
        }
    }
}