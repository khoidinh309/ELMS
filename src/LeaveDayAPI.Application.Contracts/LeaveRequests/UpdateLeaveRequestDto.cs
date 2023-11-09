using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public class UpdateLeaveRequestDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(LeaveRequestConsts.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
