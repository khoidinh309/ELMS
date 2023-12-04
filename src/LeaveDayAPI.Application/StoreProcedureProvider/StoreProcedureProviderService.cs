using LeaveDayAPI.EntityFrameworkCore;
using LeaveDayAPI.LeaveRequests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LeaveDayAPI.StoreProcedureProvider
{
    [RemoteService(false)]
    public class StoreProcedureProviderService : EfCoreRepository<LeaveDayAPIDbContext, LeaveRequest, Guid>, IStoreProcedureProviderService
    {
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public StoreProcedureProviderService(
           IRepository<IdentityUser, Guid> userRepository,
           IDbContextProvider<LeaveDayAPIDbContext> dbContextProvider
        ) : base(dbContextProvider)
        {
            this._userRepository = userRepository;
        }
        public async Task<List<LeaveRequestItemDto>> SearchProcedure(SearchLeaveRequestDto input)
        {
            var result_list = new List<LeaveRequestItemDto>();

            var commandText = "LEAVE_REQUEST_SEARCH";
            var commandType = CommandType.StoredProcedure;
            var parameter_list = new List<SqlParameter>();
            var surNameParam = new SqlParameter("@p_surname", input.Surname);
            parameter_list.Add(surNameParam);

            var emailNameParam = new SqlParameter("@p_email", input.Email);
            parameter_list.Add(emailNameParam);

            var startDateNameParam = new SqlParameter("@p_start_date", input.StartDate);
            parameter_list.Add(startDateNameParam);

            var endDateNameParam = new SqlParameter("@p_end_date", input.EndDate);
            parameter_list.Add(endDateNameParam);

            var approveStatusParam = new SqlParameter("@p_approve_status",input.ApproveStatus);
            parameter_list.Add(approveStatusParam);


            using (var command = CreateCommand(commandText, commandType, parameter_list))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    while (await dataReader.ReadAsync())
                    {
                        var idOrdinal = dataReader.GetOrdinal("Id");
                        var id = dataReader.IsDBNull(idOrdinal) ? Guid.Empty : dataReader.GetGuid(idOrdinal);

                        var leave_request = new LeaveRequestItemDto
                        {
                            Id = id,
                            SurName = dataReader.IsDBNull(dataReader.GetOrdinal("Surname"))
                                ? dataReader.GetString(dataReader.GetOrdinal("UserName"))
                                : dataReader.GetString(dataReader.GetOrdinal("Surname")) ?? "DefaultSurname",
                            Title = dataReader.IsDBNull(dataReader.GetOrdinal("Title"))
                                ? "Not available"
                                : dataReader.GetString(dataReader.GetOrdinal("Title")) ?? "DefaultTitle",
                            CreationTime = dataReader.GetDateTime(dataReader.GetOrdinal("CreationTime")),
                        };

                        result_list.Add(leave_request);

                    }
                }
            }

            return result_list;
        }

        private DbCommand CreateCommand(string commandText, CommandType commandType, List<SqlParameter> parameters)
        {
            var command = DbContext.Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = DbContext.Database.CurrentTransaction?.GetDbTransaction();

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}
