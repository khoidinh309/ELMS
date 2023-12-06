using LeaveDayAPI.LeaveRequests;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Xunit;

namespace LeaveDayAPI.LeaveRequestServices
{
    public class LeaveRequestServiceTest : LeaveDayAPIApplicationTestBase
    {
        private readonly ILeaveRequestService _leaveRequestServices;
        private readonly LeaveDayTestData _leaveDayTestData;
        private readonly IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid> _leaveRequestRepository;
        private readonly IRepository<LeaveDayAPI.LeaveRequests.LeaveDay> _leaveDayRepository;
        private ICurrentUser _currentUser;

        public LeaveRequestServiceTest()
        {
            this._leaveRequestServices = GetRequiredService<ILeaveRequestService>();
            this._leaveDayTestData = GetRequiredService<LeaveDayTestData>();
            this._leaveRequestRepository = GetRequiredService<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            this._leaveDayRepository = GetRequiredService<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();
        }

        [Fact]
        public async Task Should_Create_LeaveRequest()
        {
            // Arrange
            Login(_leaveDayTestData.UserKhoiId);
            var createLeaveRequestDto = new CreateLeaveRequestDto
            {
                Title = this._leaveDayTestData.RequestWeddingTitle,
                Reason = this._leaveDayTestData.RequestWeddingReason,
                StartDate = this._leaveDayTestData.ValidStartDate,
                EndDate = this._leaveDayTestData.ValidEndDate
            };

            // Act
            var leaveRequestDto = await _leaveRequestServices.CreateAsync(createLeaveRequestDto);

            // Assert
            leaveRequestDto.ShouldNotBeNull();
            leaveRequestDto.Id.ShouldNotBe(Guid.Empty);

            var createdLeaveRequest = await _leaveRequestRepository.GetAsync(leaveRequestDto.Id);
            createdLeaveRequest.ShouldNotBeNull();
            createdLeaveRequest.Title.ShouldBe(_leaveDayTestData.RequestWeddingTitle);

            await _leaveRequestRepository.DeleteAsync(leaveRequestDto.Id);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Invalid_Dates()
        {
            Login(_leaveDayTestData.UserKhoiId);

            var createLeaveRequestDto = new CreateLeaveRequestDto
            {
                Title = this._leaveDayTestData.RequestWeddingTitle,
                Reason = this._leaveDayTestData.RequestWeddingReason,
                StartDate = this._leaveDayTestData.InvalidEndDate_Before_Now,
                EndDate = this._leaveDayTestData.InvaidStartDate_After_End
            };

            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _leaveRequestServices.CreateAsync(createLeaveRequestDto);
            });

            exception.Message.ShouldContain("Invalid date time");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_The_Days_Number_Over()
        {
            Login(_leaveDayTestData.UserKhoiId);

            var createLeaveRequestDto = new CreateLeaveRequestDto
            {
                Title = this._leaveDayTestData.RequestWeddingTitle,
                Reason = this._leaveDayTestData.RequestWeddingReason,
                StartDate = this._leaveDayTestData.ValidStartDate,
                EndDate = this._leaveDayTestData.ValidStartDate.AddDays(13)
            };

            var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
            {
                await _leaveRequestServices.CreateAsync(createLeaveRequestDto);
            });

            exception.Message.ShouldContain("Your remaining days is not enough");
        }

        [Fact]
        public async Task Should_Delete_LeaveRequest()
        {
            Login(this._leaveDayTestData.UserKhoiId);

            // Arrange
            var existing_request = await _leaveRequestRepository.GetAsync(_leaveDayTestData.RequestDatingId);

            // Act
            
            var result = await _leaveRequestServices.DeleteAsync(existing_request.Id);

            // Assert
            result.ShouldBeTrue();

            await WithUnitOfWorkAsync(async () =>
            {
                var user_leave_day = await _leaveDayRepository.SingleOrDefaultAsync(ld => ld.UserId == existing_request.UserId);

                user_leave_day.RemainingDayNumber.ShouldBe(this._leaveDayTestData.RemainingDays + this._leaveDayTestData.Number_of_Valid_Days);
            });
        }

        [Fact]
        public async Task Should_Approve_LeaveRequest()
        {
            // Arrange
            var approveRequest = new ApproveLeaveRequestDto
            {
                Id = this._leaveDayTestData.RequestDatingId,
                ApproveStatus = ApproveStatus.IsApproved
            };

            // Act
            var result = await _leaveRequestServices.ApproveOrRejectAsync(approveRequest);

            // Assert
            result.ShouldBeTrue();
            var approved_request = await _leaveRequestRepository.GetAsync(this._leaveDayTestData.RequestDatingId);
            approved_request.ApproveStatus.ShouldBe(ApproveStatus.IsApproved);
        }

        [Fact]
        public async Task Should_Reject_LeaveRequest()
        {
            // Arrange
            var approveRequest = new ApproveLeaveRequestDto
            {
                Id = this._leaveDayTestData.RequestDatingId,
                ApproveStatus = ApproveStatus.IsRequested
            };

            // Act
            var result = await _leaveRequestServices.ApproveOrRejectAsync(approveRequest);

            // Assert
            result.ShouldBeTrue();
            var rejected_request = await _leaveRequestRepository.GetAsync(this._leaveDayTestData.RequestDatingId);
            rejected_request.ApproveStatus.ShouldBe(ApproveStatus.IsRejected);

            await WithUnitOfWorkAsync(async () =>
            {
                var user_leave_day = await _leaveDayRepository.SingleOrDefaultAsync(ld => ld.UserId == rejected_request.UserId);

                user_leave_day.RemainingDayNumber.ShouldBe(this._leaveDayTestData.RemainingDays + this._leaveDayTestData.Number_of_Valid_Days);
            });
        }

        private void Login(Guid userId)
        {
            _currentUser.Id.Returns(userId);
            _currentUser.IsAuthenticated.Returns(true);
        }

        protected override void AfterAddApplication( IServiceCollection services)
        {
            _currentUser = Substitute.For<ICurrentUser>();
            services.AddSingleton(_currentUser);
        }


    }
}
