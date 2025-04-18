using AutoMapper;
using LeaveManager.Application.DTOs;
using LeaveManager.Domain.Entities;

namespace LeaveManager.Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ReverseMap();
            CreateMap<LeaveRequest,LeaveRequestFilterDto>().ReverseMap();
            CreateMap<LeaveRequest, LeaveRequestSummaryDto>().ReverseMap();

        }
    }
   
}
