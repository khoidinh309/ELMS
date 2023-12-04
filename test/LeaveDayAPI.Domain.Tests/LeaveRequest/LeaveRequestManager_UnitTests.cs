using LeaveDayAPI.LeaveRequests;
using NSubstitute;
using Shouldly;
using System;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Volo.Abp.Domain.Repositories;

namespace LeaveDayAPI.LeaveRequest
{
    public class LeaveRequestManager_UnitTests : LeaveDayAPIDomainTestBase
    {
        [Fact]
        public void IsValidDate_True()
        {
            DateTime startDate = DateTime
                .Now.Date.AddDays(1);
            DateTime endDate = DateTime.Now.Date.AddDays(2);

            var leaveRequestManager = new LeaveRequestManager(null, null);

            var result = leaveRequestManager.IsValidDates(startDate, endDate);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void IsValidDate_False_Before_Or_Equal_Now(int number_of_day_before)
        {
            DateTime startDate = DateTime
                .Now.Date.AddDays(number_of_day_before);
            DateTime endDate = DateTime.Now.Date.AddDays(2);

            var leaveRequestManager = new LeaveRequestManager(null, null);

            var result = leaveRequestManager.IsValidDates(startDate, endDate);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidDate_False_StartDate_Is_Less_Than_End()
        {
            DateTime startDate = DateTime
                .Now.Date.AddDays(2);
            DateTime endDate = DateTime.Now.Date.AddDays(-2);

            var leaveRequestManager = new LeaveRequestManager(null, null);

            var result = leaveRequestManager.IsValidDates(startDate, endDate);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidDate_False_Same_Date_But_Before_Or_Equal_Now()
        {
            DateTime startDate = DateTime
                .Now.Date.AddDays(-1);
            DateTime endDate = DateTime.Now.Date.AddDays(-1);

            var leaveRequestManager = new LeaveRequestManager(null, null);

            var result = leaveRequestManager.IsValidDates(startDate, endDate);

            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValidDate_False_Same_Valid_Date()
        {
            DateTime startDate = DateTime
                .Now.Date.AddDays(1);
            DateTime endDate = DateTime.Now.Date.AddDays(1);

            var leaveRequestManager = new LeaveRequestManager(null, null);

            var result = leaveRequestManager.IsValidDates(startDate, endDate);

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ApproveAsync_LeaveRequestNotFound_ReturnsFalse()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(Task.FromResult<LeaveDayAPI.LeaveRequests.LeaveRequest>(null));

            var leaveDayManager = Substitute.For<ILeaveDayManager>();

            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.ApproveAsync(leaveRequestId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ApproveAsync_RejectedLeaveRequest_ReturnsFalse()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(new LeaveDayAPI.LeaveRequests.LeaveRequest { ApproveStatus = ApproveStatus.IsRejected });

            var leaveDayManager = Substitute.For<ILeaveDayManager>();

            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.ApproveAsync(leaveRequestId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ApproveAsync_SuccessfulApproval_ReturnsTrue()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(new LeaveDayAPI.LeaveRequests.LeaveRequest { ApproveStatus = ApproveStatus.IsRequested, StartDate = DateTime.Now.Date.AddDays(1), EndDate = DateTime.Now.Date.AddDays(2) });
            var leaveDayManager = Substitute.For<ILeaveDayManager>();
            leaveDayManager.UpdateRemainingDay(Arg.Any<Guid>(), Arg.Any<int>()).Returns(true);

            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.ApproveAsync(leaveRequestId);

            // Assert
            Assert.True(result);
            await leaveRequestRepository.Received().UpdateAsync(Arg.Is<LeaveDayAPI.LeaveRequests.LeaveRequest>(lr => lr.ApproveStatus == ApproveStatus.IsApproved));
        }

        [Fact]
        public async Task RejectAsync_SuccessfulRejection_ReturnsTrue()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(new LeaveDayAPI.LeaveRequests.LeaveRequest { ApproveStatus = ApproveStatus.IsRequested });
            var leaveDayManager = Substitute.For<ILeaveDayManager>();
            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.RejectAsync(leaveRequestId);

            // Assert
            Assert.True(result);
            await leaveRequestRepository.Received().UpdateAsync(Arg.Is<LeaveDayAPI.LeaveRequests.LeaveRequest>(lr => lr.ApproveStatus == ApproveStatus.IsRejected));
        }

        [Fact]
        public async Task RejectAsync_LeaveRequestNotFound_ReturnsFalse()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(Task.FromResult<LeaveDayAPI.LeaveRequests.LeaveRequest>(null));
            var leaveDayManager = Substitute.For<ILeaveDayManager>();
            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.RejectAsync(leaveRequestId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RejectAsync_AlreadyApproved_ReturnsFalse()
        {
            // Arrange
            var leaveRequestId = Guid.NewGuid();
            var leaveRequestRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveRequest, Guid>>();
            leaveRequestRepository.GetAsync(leaveRequestId).Returns(new LeaveDayAPI.LeaveRequests.LeaveRequest { ApproveStatus = ApproveStatus.IsApproved });
            var leaveDayManager = Substitute.For<ILeaveDayManager>();
            var leaveRequestManager = new LeaveRequestManager(leaveRequestRepository, leaveDayManager);

            // Act
            var result = await leaveRequestManager.RejectAsync(leaveRequestId);

            // Assert
            Assert.False(result);
            await leaveRequestRepository.DidNotReceive().UpdateAsync(Arg.Any<LeaveDayAPI.LeaveRequests.LeaveRequest>());
        }
    }
}
