using LeaveDayAPI.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace LeaveDayAPI;

[DependsOn(
    typeof(LeaveDayAPIEntityFrameworkCoreTestModule),
    typeof(LeaveDayAPIDomainModule)
    )]
public class LeaveDayAPIDomainTestModule : AbpModule
{

}
