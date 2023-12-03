using LeaveDayAPI.LeaveRequests;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace LeaveDayAPI.LeaveDay
{
    public class LeaveDayManager_UnitTests : LeaveDayAPIDomainTestBase
    {
        [Fact]
        public void IsEnoughRemainingDays()
        {
            var userId = Guid.NewGuid();

            var leave_day = new LeaveDayAPI.LeaveRequests.LeaveDay
            {
                UserId = userId,
                RemainingDayNumber = 12
            };

            var leaveDayRepository = Substitute.For<IRepository<LeaveDayAPI.LeaveRequests.LeaveDay>>();

            var leaveDayManager = new LeaveDayManager(leaveDayRepository);
            leaveDayManager.GetRemainingDayNumberAsync(userId).Returns(2);


        }
    }
}
