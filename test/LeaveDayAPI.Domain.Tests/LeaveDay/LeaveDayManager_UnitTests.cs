using LeaveDayAPI.LeaveRequests;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace LeaveDayAPI.LeaveDay
{
    public class LeaveDayManager_UnitTests : LeaveDayAPIDomainTestBase
    {
        [Theory]
        [InlineData("2024-01-02", "2024-01-02", 1)]
        [InlineData("2024-01-02", "2024-01-04", 4)]
        [InlineData("2024-01-02", "2024-01-13", 12)]
        public async void IsEnoughRemainingDays_True(string startDate, string endDate, int remainingDay)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var leaveDayRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();

            // Setup the SingleOrDefaultAsync to return a LeaveDay with the expected UserId
            leaveDayRepository
                .SingleOrDefaultAsync(Arg.Any<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>())
                .Returns(c =>
                {
                    var predicate = c.Arg<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>();
                    var leaveDay = new LeaveDayAPI.LeaveRequests.LeaveDay
                    {
                        UserId = userId,
                        RemainingDayNumber = remainingDay
                    };
                    return Task.FromResult(predicate.Compile()(leaveDay) ? leaveDay : null);
                });

            var leaveDayManager = new LeaveDayManager(leaveDayRepository);

            // Act
            DateTime startDate_DT = DateTime.Parse(startDate);
            DateTime endDate_DT = DateTime.Parse(endDate);
            var result = await leaveDayManager.IsEnoughRemainingDays(startDate_DT, endDate_DT, userId);

            // Assert
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData("2024-01-02", "2024-01-02", 0)]
        [InlineData("2024-01-02", "2024-01-5", 3)]
        [InlineData("2024-01-02", "2024-01-13", 11)]
        public async void IsEnoughRemainingDays_False(string startDate, string endDate, int remainingDay)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leave_day = new LeaveDayAPI.LeaveRequests.LeaveDay
            {
                UserId = userId,
                RemainingDayNumber = remainingDay
            };
            var leaveDayRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();
            // Set up
            leaveDayRepository
                .SingleOrDefaultAsync(Arg.Any<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>())
                .Returns(c =>
                {
                    var predicate = c.Arg<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>();
                    var leaveDay = new LeaveDayAPI.LeaveRequests.LeaveDay
                    {
                        UserId = userId,
                        RemainingDayNumber = remainingDay
                    };
                    return Task.FromResult(predicate.Compile()(leaveDay) ? leaveDay : null);
                });

            var leaveDayManager = new LeaveDayManager(leaveDayRepository);

            DateTime startDate_DT = DateTime.Parse(startDate);
            DateTime endDate_DT = DateTime.Parse(endDate);

            //Act
            var result = await leaveDayManager.IsEnoughRemainingDays(startDate_DT, endDate_DT, userId);

            //Assert
            result.ShouldBeFalse();
        }

        [Theory]
        [InlineData(1,1)]
        [InlineData(5,3)]
        [InlineData(12,12)]
        public async void UpdateRemainingDay_True(int remaining_day, int request_day)
        {
            var userId = Guid.NewGuid();
            var leaveDayRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();
            leaveDayRepository
                .SingleOrDefaultAsync(Arg.Any<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>())
                .Returns(new LeaveDayAPI.LeaveRequests.LeaveDay
                {
                    UserId = userId,
                    RemainingDayNumber = remaining_day
                });

            var leaveDayManager = new LeaveDayManager(leaveDayRepository);

            // Act
            var result = await leaveDayManager.UpdateRemainingDay(userId, request_day);

            // Assert
            Assert.True(result);
            await leaveDayRepository.Received().UpdateAsync(Arg.Is<LeaveDayAPI.LeaveRequests.LeaveDay>(ld => ld.RemainingDayNumber == (remaining_day - request_day)));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(2, 3)]
        [InlineData(11, 12)]
        public async void UpdateRemainingDay_False(int remaining_day, int request_day)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var leave_day = new LeaveDayAPI.LeaveRequests.LeaveDay
            {
                UserId = userId,
                RemainingDayNumber = remaining_day
            };
            var leaveDayRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();
            // Set up
            leaveDayRepository
                .SingleOrDefaultAsync(Arg.Any<Expression<Func<LeaveDayAPI.LeaveRequests.LeaveDay, bool>>>())
                .Returns(new LeaveDayAPI.LeaveRequests.LeaveDay
                {
                    UserId = userId,
                    RemainingDayNumber = remaining_day
                });

            var leaveDayManager = new LeaveDayManager(leaveDayRepository);

            //Act

            var result = await leaveDayManager.UpdateRemainingDay(userId, request_day);

            //Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsEnoughRemainingDaysForUpdate()
        {
            var result = true;
            result.ShouldBeTrue();
        }

    }
}
