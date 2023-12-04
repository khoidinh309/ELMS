using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace LeaveDayAPI.LeaveRequests
{
    public interface ILeaveDayManager : IDomainService
    {
        public Task<int> GetRemainingDayNumberAsync(Guid userId);
        public Task<bool> IsEnoughRemainingDays(DateTime startDate, DateTime endDate, Guid userId);
        public Task<bool> IsEnoughRemainingDaysForUpdate(LeaveRequest old_lr, DateTime startDate, DateTime endDate, Guid userId);
        public Task<bool> UpdateRemainingDay(Guid userId, int number_of_days);
        public Task<bool> UpdateRemainingDayForUpdate(LeaveRequest old_lr, int number_of_days);
        public Task<bool> Return_Days(LeaveRequest lr);
    }
}
