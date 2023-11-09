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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace LeaveDayAPI.LeaveRequests
{
    [Authorize]
    public class RequestLeaveService : LeaveDayAPIAppService, ILeaveRequestService
    {
        private readonly IRepository<LeaveRequest, Guid> _leaveRequestRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IStoreProcedureProviderService _storeProcedureProvider;

        public RequestLeaveService(IRepository<LeaveRequest, Guid> leaveRequestRepository
                                    ,IRepository<IdentityUser, Guid> userRepository
        )
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._userRepository = userRepository;
        }

        [Authorize(LeaveDayAPIPermissions.ApprovePermission)]
        public async Task<LeaveRequestDto> ApproveAsync(Guid id)
        {
            var leave_request = await _leaveRequestRepository.GetAsync(id);

            if(leave_request == null ) 
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            if(leave_request.ApproveStatus == ApproveStatus.IsRejected)
            {
                throw new UserFriendlyException(L["LeaveRequest:ApproveError"]);
            }

            leave_request.ApproveStatus = ApproveStatus.IsApproved;

            await _leaveRequestRepository.UpdateAsync(leave_request);
            await CurrentUnitOfWork.SaveChangesAsync();

            var leave_request_dto = ObjectMapper.Map<LeaveRequest, LeaveRequestDto>(leave_request);
            var request_user = await _userRepository.GetAsync(leave_request.UserId);

            leave_request_dto.Surname = request_user.Surname;

            return leave_request_dto;
        }

        [Authorize(LeaveDayAPIPermissions.CreatePermission)]
        public async Task<LeaveRequestDto> CreateAsync(CreateLeaveRequestDto leaveRequest)
        {
            if(leaveRequest.StartDate > leaveRequest.EndDate)
            {
                throw new UserFriendlyException(L["LeaveRequest:InValidDate"]);
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

        [Authorize(LeaveDayAPIPermissions.DeletePermission)]
        public async Task<bool> DeleteAsync(Guid Id)
        {
            var leave_request = _leaveRequestRepository.GetAsync(Id);

            if(leave_request == null)
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            await _leaveRequestRepository.DeleteAsync(Id);

            return true;
        }

        [Authorize(LeaveDayAPIPermissions.ViewPermission)]
        public async Task<List<LeaveRequestItemDto>> GetUserRequestAsync(GetLeaveRequestDto userInfo)
        {
            var request_user = await _userRepository
                .SingleOrDefaultAsync(u => u.Surname == userInfo.Surname && u.Email == userInfo.Email);

            if(request_user == null)
            {
                throw new UserFriendlyException(L["User:NotFound"]);
            }

            var request_list = await _leaveRequestRepository
                .GetListAsync(lr => lr.UserId == request_user.Id);

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
            if (leaveRequest.StartDate > leaveRequest.EndDate)
            {
                throw new UserFriendlyException(L["LeaveRequest:InValidDate"]);
            }

            var leave_request = await _leaveRequestRepository.GetAsync(leaveRequest.Id);

            if (leave_request == null)
            {
                throw new UserFriendlyException(L["LeaveRequest:NotFound"]);
            }

            if (leave_request.ApproveStatus == ApproveStatus.IsRequested)
            {
                throw new UserFriendlyException(L["LeaveRequest:UpdateError"]);
            }
   
            leave_request.Title = leaveRequest.Title;
            leave_request.Reason = leaveRequest.Reason;
            leave_request.StartDate = leaveRequest.StartDate;
            leave_request.EndDate = leaveRequest.EndDate;
            leave_request.ApproveStatus = ApproveStatus.IsRequested;
            
            var leave_request_dto = ObjectMapper.Map<LeaveRequest, LeaveRequestDto>(leave_request);
            var request_user = await _userRepository.GetAsync(leave_request.UserId);

            leave_request_dto.Surname = request_user.Surname;

            await _leaveRequestRepository.UpdateAsync(leave_request);
            await CurrentUnitOfWork.SaveChangesAsync();

            return leave_request_dto;
        }
    }
}
