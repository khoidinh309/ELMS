using LeaveDayAPI.LeaveRequests;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace LeaveDayAPI;

public class LeaveDayAPITestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly LeaveDayTestData _leaveDayTestData;
    private readonly IRepository<LeaveRequest, Guid> _leaveRequestRepository;
    private readonly IRepository<LeaveDay> _leaveDayRepository;

    public LeaveDayAPITestDataSeedContributor(IIdentityUserRepository userRepository
        , LeaveDayTestData leaveDayTestData
        , IRepository<LeaveRequest, Guid> leaveRequestRepository
        , IRepository<LeaveDay> leaveDayRepository
        )
    {
        this._leaveDayTestData = leaveDayTestData;
        this._leaveRequestRepository = leaveRequestRepository;
        this._userRepository = userRepository;
        this._leaveDayRepository = leaveDayRepository;
    }
    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateUsersAsync();
        await CreateRequestAsync();
        await CreateLeaveDayAsync();
    }

    private async Task CreateUsersAsync()
    {
        await _userRepository.InsertAsync(
            new IdentityUser(_leaveDayTestData.ManagerId,
                _leaveDayTestData.ManagerUserName,
                "thuan.le@gmail.com"
            ));
    }

    private async Task CreateRequestAsync()
    {
        await _leaveRequestRepository.InsertAsync(
            new LeaveRequest(_leaveDayTestData.RequestDatingId,
                _leaveDayTestData.RequestDatingTitle,
                _leaveDayTestData.RequestDatingReason,
                _leaveDayTestData.ValidStartDate, _leaveDayTestData.ValidEndDate,
                ApproveStatus.IsRequested,
                _leaveDayTestData.UserKhoiId
            )) ;
    }

    private async Task CreateLeaveDayAsync()
    {
        await _leaveDayRepository.InsertAsync(
            new LeaveDay
            {
                UserId = _leaveDayTestData.UserKhoiId,
                RemainingDayNumber = _leaveDayTestData.RemainingDays
            });
    }

}
