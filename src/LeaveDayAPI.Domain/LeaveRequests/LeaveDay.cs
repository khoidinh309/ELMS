using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveDay : Entity<Guid>
    {
        public Guid UserId { get; set; }
        public int RemainingDayNumber { get; set;}
    }
}
