using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveDayManager : DomainService
    {
        private readonly IRepository<LeaveDay> _leaveDayRepository;

        public LeaveDayManager(IRepository<LeaveDay> leaveDayRepositoy)
        {
            this._leaveDayRepository = leaveDayRepositoy;
        }

        public async Task<int> GetRemainingDayNumberAsync(Guid userId)
        {
            var user = await _leaveDayRepository.SingleOrDefaultAsync(x => x.UserId == userId);
            return user?.RemainingDayNumber ?? -1;
        }

        public async Task<bool> IsEnoughRemainingDays(DateTime startDate, DateTime endDate, Guid userId)
        {
            var request_day_number = endDate.Subtract(startDate).Days + 1;
            var remaining_days = await this.GetRemainingDayNumberAsync(userId);

            return remaining_days > request_day_number;
        }

        public async Task<bool> UpdateRemainingDay(Guid userId, int number_of_days)
        {
            var user_remaining_day = await _leaveDayRepository.SingleOrDefaultAsync(x => x.UserId == userId);
            
            if(user_remaining_day == null)
            {
                return false;
            }

            if(user_remaining_day.RemainingDayNumber < number_of_days)
            {
                return false;
            }

            user_remaining_day.RemainingDayNumber -= number_of_days;
            await _leaveDayRepository.UpdateAsync(user_remaining_day);
            
            return true;
        }
    }
}
