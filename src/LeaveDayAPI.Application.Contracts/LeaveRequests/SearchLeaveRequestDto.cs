using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public class SearchLeaveRequestDto
    {
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ApproveStatus? ApproveStatus { get; set; }
    }
}
