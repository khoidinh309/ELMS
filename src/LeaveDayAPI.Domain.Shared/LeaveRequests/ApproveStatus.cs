using System;
using System.Collections.Generic;
using System.Text;

namespace LeaveDayAPI.LeaveRequests
{
    public enum ApproveStatus : byte
    {
        IsRequested,
        IsApproved,
        IsRejected
    }
}
