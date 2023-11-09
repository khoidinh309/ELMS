using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Auditing;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveRequestItemDto : IHasCreationTime
    {
        public Guid Id { get; set; }
        public string SurName { get; set; }
        public string Title { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
