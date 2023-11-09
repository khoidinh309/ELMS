using System.Threading.Tasks;

namespace LeaveDayAPI.Data;

public interface ILeaveDayAPIDbSchemaMigrator
{
    Task MigrateAsync();
}
