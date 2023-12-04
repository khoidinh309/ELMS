using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public class ApproveLeaveRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public ApproveStatus ApproveStatus { get; set; }
    }
}
