using Volo.Abp.Modularity;

namespace LeaveDayAPI;

[DependsOn(
    typeof(LeaveDayAPIApplicationModule),
    typeof(LeaveDayAPIDomainTestModule)
    )]
public class LeaveDayAPIApplicationTestModule : AbpModule
{

}
