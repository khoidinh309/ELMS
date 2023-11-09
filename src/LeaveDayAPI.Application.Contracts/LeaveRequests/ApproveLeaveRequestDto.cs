using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public class ApproveLeaveRequestDto
    {
        [Required]
        public string Surname { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(LeaveRequestConsts.MaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }
    }
}
