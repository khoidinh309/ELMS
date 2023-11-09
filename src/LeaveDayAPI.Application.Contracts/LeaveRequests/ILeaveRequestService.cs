using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeaveDayAPI.LeaveRequests
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto leaveRequest);
        Task<LeaveRequestDto> UpdateAsync(UpdateLeaveRequestDto leaveRequest);
        Task<bool> DeleteAsync(Guid Id);
        Task<List<LeaveRequestItemDto>> SearchAsync(SearchLeaveRequestDto input);
        Task<List<LeaveRequestItemDto>> GetUserRequestAsync(GetLeaveRequestDto userInfo);
        Task<LeaveRequestDto> ApproveAsync(Guid id);
    }
}
