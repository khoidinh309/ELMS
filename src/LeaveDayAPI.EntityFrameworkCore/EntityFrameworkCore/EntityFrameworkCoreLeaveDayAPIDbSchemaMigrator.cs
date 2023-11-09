using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LeaveDayAPI.Data;
using Volo.Abp.DependencyInjection;

namespace LeaveDayAPI.EntityFrameworkCore;

public class EntityFrameworkCoreLeaveDayAPIDbSchemaMigrator
    : ILeaveDayAPIDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreLeaveDayAPIDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the LeaveDayAPIDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<LeaveDayAPIDbContext>()
            .Database
            .MigrateAsync();
    }
}
