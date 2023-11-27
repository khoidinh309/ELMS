using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public class SearchLeaveRequestDto
    {
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ApproveStatus { get; set; }
    }
}
