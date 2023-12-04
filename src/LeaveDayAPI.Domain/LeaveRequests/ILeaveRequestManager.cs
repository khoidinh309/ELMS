using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace LeaveDayAPI.LeaveRequests
{
    public interface ILeaveRequestManager : IDomainService
    {
        public bool IsValidDates(DateTime startDate, DateTime endDate);
        public void UpdateAsync(LeaveRequest @leave_request, string title, string reason, DateTime startDate, DateTime endDate);
        public Task<bool> ApproveAsync(Guid leave_request_id);
        public Task<bool> RejectAsync(Guid leave_request_id);
    }
}
