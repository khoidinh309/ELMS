using LeaveDayAPI.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace LeaveDayAPI;

[DependsOn(
    typeof(LeaveDayAPIEntityFrameworkCoreTestModule)
    )]
public class LeaveDayAPIDomainTestModule : AbpModule
{

}
