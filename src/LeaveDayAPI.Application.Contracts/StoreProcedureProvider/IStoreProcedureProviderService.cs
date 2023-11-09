using LeaveDayAPI.LeaveRequests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeaveDayAPI.StoreProcedureProvider
{
    public interface IStoreProcedureProviderService
    {
        Task<List<LeaveRequestItemDto>> SearchProcedure(SearchLeaveRequestDto input);
    }
}
