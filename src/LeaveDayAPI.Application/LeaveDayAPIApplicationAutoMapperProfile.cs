using AutoMapper;
using LeaveDayAPI.LeaveRequests;

namespace LeaveDayAPI;

public class LeaveDayAPIApplicationAutoMapperProfile : Profile
{
    public LeaveDayAPIApplicationAutoMapperProfile()
    {
        CreateMap<LeaveRequest, LeaveRequestDto>();
        CreateMap<LeaveRequest, LeaveRequestItemDto>();
    }
}
