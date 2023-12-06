using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveRequest : AuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ApproveStatus ApproveStatus { get; set; }

        public LeaveRequest()
        {
            
        }

        public LeaveRequest(Guid id, string title, string reason, 
            DateTime startDate, 
            DateTime endDate,
            ApproveStatus approveStatus,
            Guid userId
        ) : base(id)
        {
            this.Title = title;
            this.Reason = reason;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ApproveStatus = approveStatus;
            this.UserId = userId;
        }

    }
}
