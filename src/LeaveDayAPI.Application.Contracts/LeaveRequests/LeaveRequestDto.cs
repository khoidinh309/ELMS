using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveRequestDto : AuditedEntityDto<Guid>
    {
        public string Surname { get; set; }
        public string  Email { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ApproveStatus ApproveStatus { get; set; }
    }
}
