using Azure.Core;
using LeaveDayAPI.Permissions;
using LeaveDayAPI.StoreProcedureProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Clients;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace LeaveDayAPI.LeaveRequests
{
    [RemoteService(false)]
    [Authorize]
    public class LeaveRequestService : LeaveDayAPIAppService, ILeaveRequestService
    {
        private readonly LeaveDayManager _leaveDayManager;
        private readonly LeaveRequestManager _leaveRequestManager;
        private readonly IRepository<LeaveRequest, Guid> _leaveRequestRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IStoreProcedureProviderService _storeProcedureProvider;
        private readonly IRepository<LeaveDay> _leaveDayRepository;

        public LeaveRequestService(IRepository<LeaveRequest, Guid> leaveRequestRepository
                                    ,IRepository<IdentityUser, Guid> userRepository
                                    ,IRepository<LeaveDay> leaveDayRepository
                                    , IStoreProcedureProviderService storeProcedureProvide
                                    , LeaveDayManager leaveDayManager
                                    , LeaveRequestManager leaveRequestManager
        )
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._userRepository = userRepository;
            this._leaveDayRepository = leaveDayRepository;
            this._storeProcedureProvider = storeProcedureProvide;
            this._leaveDayManager = leaveDayManager;
            this._leaveRequestManager = leaveRequestManager;
        }

        [Authorize(LeaveDayAPIPermissions.ApprovePermission)]
        public async Task<bool> ApproveOrRejectAsync(ApproveLeaveRequestDto request)
        {
            if(request == null)
            {
                throw new UserFriendlyException(L["LeaveReques:InvalidRequest"]);
            }

            if (request.ApproveStatus == ApproveStatus.IsApproved)
            {
                var approve_result =  await this._leaveRequestManager.ApproveAsync(request.Id);
                if(!approve_result)
                {
                    throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
                }
                return approve_result;
            }
            else
            {
                var reject_result = await this._leaveRequestManager.RejectAsync(request.Id);
                if(!reject_result)
                {
                    throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
                }
                return reject_result;
            }
        }

        [Authorize(LeaveDayAPIPermissions.CreatePermission)]
        public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto leaveRequest)
        {
            try
            {
                if (this._leaveRequestManager.IsValidDates(leaveRequest.StartDate, leaveRequest.EndDate) == false)
                {
                    throw new UserFriendlyException(L["LeaveRequest:InValidDate"]);
                }

                if (CurrentUser == null)
                {
                    throw new UserFriendlyException(L["User:NotFound"]);
                }

                var userId = (Guid)CurrentUser.Id;

                if (await this._leaveDayManager.IsEnoughRemainingDays(leaveRequest.StartDate, leaveRequest.EndDate, userId) == false)
                {
                    var user_remaining_days_number = await this._leaveDayManager.GetRemainingDayNumberAsync(userId);
                    throw new UserFriendlyException(L["LeaveRequest:NotEnoughRemainingDay", user_remaining_days_number]);
                }

                var leave_request = new LeaveRequest
                {
                    Title = leaveRequest.Title,
                    Reason = leaveRequest.Reason,
                    StartDate = leaveRequest.StartDate,
                    EndDate = leaveRequest.EndDate,
                    UserId = (Guid)CurrentUser.Id,
                    ApproveStatus = ApproveStatus.IsRequested
                };

                await _leaveRequestRepository.InsertAsync(leave_request);
                await CurrentUnitOfWork.SaveChangesAsync();

                var leave_request_dto = ObjectMapper.Map<LeaveRequest, LeaveRequestDto>(leave_request);

                leave_request_dto.Surname = CurrentUser.SurName;
                leave_request_dto.Email = CurrentUser.Email;

                return leave_request_dto;
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(ex.ToString());
            }
        }

        [Authorize(LeaveDayAPIPermissions.DeletePermission)]
        public async Task<bool> DeleteAsync(Guid Id)
        {
            var leave_request = await _leaveRequestRepository.GetAsync(Id);

            if(leave_request == null)
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            await _leaveRequestRepository.DeleteAsync(Id);

            return true;
        }

        [Authorize(LeaveDayAPIPermissions.ViewPermission)]
        public async Task<List<LeaveRequestItemDto>> GetUserRequestAsync(Guid userId)
        {
            var request_user = await _userRepository.FindAsync(userId);

            if(request_user == null)
            {
                throw new UserFriendlyException(L["User:NotFound"]);
            }

            var request_list = await _leaveRequestRepository
                .GetListAsync(lr => lr.UserId == userId);

            var request_list_Dto = ObjectMapper
                .Map<List<LeaveRequest>, List<LeaveRequestItemDto>>(request_list);

            for(var i = 0; i< request_list_Dto.Count; i++)
            {
                request_list_Dto[i].SurName = request_user.Surname;
            }

            return request_list_Dto;
        }

        [Authorize(LeaveDayAPIPermissions.ViewPermission)]
        public async Task<List<LeaveRequestItemDto>> SearchAsync(SearchLeaveRequestDto input)
        {
            List<LeaveRequestItemDto> search_result = await _storeProcedureProvider.SearchProcedure(input);

            return search_result;
        }

        [Authorize(LeaveDayAPIPermissions.UpdatePermission)]
        public async Task<LeaveRequestDto> UpdateAsync(UpdateLeaveRequestDto leaveRequest)
        {
            if (this._leaveRequestManager.IsValidDates(leaveRequest.StartDate, leaveRequest.EndDate) == false)
            {
                throw new UserFriendlyException(L["LeaveRequest:InValidDate"]);
            }

            var @leave_request = await _leaveRequestRepository.GetAsync(leaveRequest.Id);

            if (@leave_request == null)
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            var userId = (Guid)CurrentUser.Id;

            if (await this._leaveDayManager.IsEnoughRemainingDays(leaveRequest.StartDate, leaveRequest.EndDate, userId) == false)
            {
                var user_remaining_days_number = await this._leaveDayManager.GetRemainingDayNumberAsync(userId);
                throw new UserFriendlyException(L["LeaveRequest:NotEnoughRemainingDay", user_remaining_days_number]);
            }

            this._leaveRequestManager.UpdateAsync(@leave_request, leaveRequest.Title, 
                    leaveRequest.Reason, leaveRequest.StartDate, leaveRequest.EndDate);

            var leave_request_dto = await this.BuildLeaveRequestDTO(@leave_request);

            await _leaveRequestRepository.UpdateAsync(@leave_request);
            await CurrentUnitOfWork.SaveChangesAsync();

            return leave_request_dto;
        }

        [Authorize(LeaveDayAPIPermissions.ApprovePermission)]
        public async Task<bool> MultipleApproveAsync(List<ApproveLeaveRequestDto> request_list)
        {
            if (request_list == null)
            {
                throw new UserFriendlyException(L["LeaveReques:InvalidRequest"]);
            }

            foreach (var request in request_list)
            {
                var approve_reject_result = await this.ApproveOrRejectAsync(request);

                if (!approve_reject_result)
                {
                    throw new UserFriendlyException("Internal Error, please try again");
                }
            }

            return true;
        }

        public async Task<int> GetRemainingDayNumberAsync(Guid userId)
        {
            return await this._leaveDayManager.GetRemainingDayNumberAsync(userId);
        }

        public async Task<LeaveRequestDto> ViewRequestDetail(Guid request_id)
        {
            var leave_request = await this._leaveRequestRepository.GetAsync(request_id);

            if(leave_request == null)
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            return await this.BuildLeaveRequestDTO(leave_request);
        }

        #region private method

        private async Task<LeaveRequestDto> BuildLeaveRequestDTO(LeaveRequest leave_request)
        {
            var leave_request_dto = ObjectMapper.Map<LeaveRequest, LeaveRequestDto>(leave_request);
            var request_user = await _userRepository.GetAsync(leave_request.UserId);

            leave_request_dto.Surname = request_user.Surname;
            leave_request_dto.Email = request_user.Email;

            return leave_request_dto;
        }

        #endregion
    }
}
