using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace LeaveDayAPI.Data;

/* This is used if database provider does't define
 * ILeaveDayAPIDbSchemaMigrator implementation.
 */
public class NullLeaveDayAPIDbSchemaMigrator : ILeaveDayAPIDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
