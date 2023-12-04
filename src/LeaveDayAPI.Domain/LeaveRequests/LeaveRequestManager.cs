using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace LeaveDayAPI.LeaveRequests
{
    public class LeaveRequestManager : DomainService
    {
        private readonly IRepository<LeaveRequest, Guid> _leaveRequestRepository;
        private readonly ILeaveDayManager _leaveDayManager;

        public LeaveRequestManager(IRepository<LeaveRequest, Guid> leaveRequestRepository
            , ILeaveDayManager leaveDayManager)
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._leaveDayManager = leaveDayManager;
        }

        public bool IsValidDates(DateTime startDate, DateTime endDate)
        {
            if (startDate < DateTime.Now || startDate > endDate)
            {
                return false;
            }

            return true;
        }

        public void UpdateAsync(LeaveRequest @leave_request, string title, string reason, DateTime startDate, DateTime endDate) 
        {
            @leave_request.Title = title;
            @leave_request.Reason = reason;
            @leave_request.StartDate = startDate;
            @leave_request.EndDate = endDate;
            @leave_request.ApproveStatus = ApproveStatus.IsRequested;
        }

        public async Task<bool> ApproveAsync(Guid leave_request_id)
        {
            var leave_request = await _leaveRequestRepository.GetAsync(leave_request_id);

            if (leave_request == null)
            {
                return false;
            }

            if (leave_request.ApproveStatus == ApproveStatus.IsRejected)
            {
                return false;
            }

            leave_request.ApproveStatus = ApproveStatus.IsApproved;

            await _leaveRequestRepository.UpdateAsync(leave_request);

            return true;
        }

        public async Task<bool> RejectAsync(Guid leave_request_id)
        {
            var leave_request = await _leaveRequestRepository.GetAsync(leave_request_id);

            if (leave_request == null)
            {
                return false;
            }

            if (leave_request.ApproveStatus == ApproveStatus.IsApproved)
            {
                return false;
            }

            if(await this._leaveDayManager.Return_Days(leave_request) == false)
            {
                return false;
            }

            leave_request.ApproveStatus = ApproveStatus.IsRejected;

            await _leaveRequestRepository.UpdateAsync(leave_request);

            return leave_request != null;
        }
    }
}
