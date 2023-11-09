﻿using LeaveDayAPI.LeaveRequests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveDayAPI.Controllers.LeaveRequests
{
    public class LeaveRequestController : LeaveDayAPIController
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            this._leaveRequestService = leaveRequestService;
        }

        [HttpPost]
        public async Task<List<LeaveRequestItemDto>> GetRequestListOfUser(GetLeaveRequestDto userInfo)
        {
            return await _leaveRequestService.GetUserRequestAsync(userInfo);
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
        public async Task<LeaveRequestDto> ApproveRequest(Guid id)
        {
            return await _leaveRequestService.ApproveAsync(id);
        }
    }
}
