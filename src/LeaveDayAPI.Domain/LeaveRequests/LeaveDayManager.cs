using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveDayManager : DomainService, ILeaveDayManager
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

            return remaining_days >= request_day_number;
        }

        public async Task<bool> IsEnoughRemainingDaysForUpdate(LeaveRequest old_lr, DateTime startDate, DateTime endDate, Guid userId)
        {
            var user_remaining_days = await this.GetRemainingDayNumberAsync(old_lr.UserId);
            var old_days = this.CalculateDayNumber(old_lr.StartDate, old_lr.EndDate);
            var new_days = this.CalculateDayNumber(startDate, endDate);

            return new_days <= old_days + user_remaining_days;
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

        public async Task<bool> UpdateRemainingDayForUpdate(LeaveRequest old_lr, int number_of_days)
        {
            var old_days = this.CalculateDayNumber(old_lr.StartDate, old_lr.EndDate);
            var user_leave_day = await _leaveDayRepository.SingleOrDefaultAsync(x => x.UserId == old_lr.UserId);

            if (user_leave_day == null)
            {
                return false;
            }

            if (old_lr.ApproveStatus == ApproveStatus.IsRequested) {
                user_leave_day.RemainingDayNumber = user_leave_day.RemainingDayNumber + old_days - number_of_days;
            }
            else
            {
                user_leave_day.RemainingDayNumber -= number_of_days;
            }

            await this._leaveDayRepository.UpdateAsync(user_leave_day);

            return true;
        }

        public async Task<bool> Return_Days(LeaveRequest lr)
        {
            var user_leave_day = await this._leaveDayRepository.SingleOrDefaultAsync(ld => ld.UserId == lr.UserId);

            if (user_leave_day == null) { return false; }

            var number_of_days = this.CalculateDayNumber(lr.StartDate, lr.EndDate);
            user_leave_day.RemainingDayNumber += number_of_days;

            await this._leaveDayRepository.UpdateAsync(user_leave_day);

            return true;

        }

        private int CalculateDayNumber(DateTime startDate,  DateTime endDate)
        {
            return endDate.Subtract(startDate).Days + 1;
        }
    }
}
