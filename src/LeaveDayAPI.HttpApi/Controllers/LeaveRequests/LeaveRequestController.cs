using Azure.Core;
using LeaveDayAPI.LeaveRequests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveDayAPI.Controllers.LeaveRequests
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class LeaveRequestController : LeaveDayAPIController
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            this._leaveRequestService = leaveRequestService;
        }

        [HttpPost]
        public async Task<List<LeaveRequestItemDto>> GetRequestListOfUser(Guid Id)
        {
            return await _leaveRequestService.GetUserRequestAsync(Id);
        }

        [HttpPost]
        public async Task<LeaveRequestDto> CreateRequest(CreateLeaveRequestDto leaveRequest)
        {
            return await _leaveRequestService.CreateAsync(leaveRequest);
        }

        [HttpPatch]
        public async Task<LeaveRequestDto> UpdateRequest(UpdateLeaveRequestDto leaveRequest)
        {
            return await _leaveRequestService.UpdateAsync(leaveRequest);
        }

        [HttpDelete]
        public async Task<bool> DeleteAsync(Guid Id)
        {
            return await _leaveRequestService.DeleteAsync(Id);
        }

        [HttpPost]
        public async Task<List<LeaveRequestItemDto>> SearchRequest(SearchLeaveRequestDto input)
        {
            return await _leaveRequestService.SearchAsync(input);
        }

        [HttpPost]
        public async Task<bool> ApproveOrRejectRequest(ApproveLeaveRequestDto request)
        {
            return await _leaveRequestService.ApproveOrRejectAsync(request);
        }

        [HttpPost]
        public async Task<bool> MultipleApproveOrRejectRequests(List<ApproveLeaveRequestDto> request_list)
        {
            return await _leaveRequestService.MultipleApproveAsync(request_list);
        }

        [HttpGet]
        public async Task<int> GetRemainingDayNumber(Guid userId)
        {
            return await _leaveRequestService.GetRemainingDayNumberAsync(userId);
        }

        [HttpGet]
        public async Task<LeaveRequestDto> ViewRequestDetail(Guid request_id)
        {
            return await this._leaveRequestService.ViewRequestDetail(request_id);
        }
    }
}
