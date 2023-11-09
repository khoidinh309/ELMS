using LeaveDayAPI.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace LeaveDayAPI.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LeaveDayAPIEntityFrameworkCoreModule),
    typeof(LeaveDayAPIApplicationContractsModule)
    )]
public class LeaveDayAPIDbMigratorModule : AbpModule
{
}
