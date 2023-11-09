using LeaveDayAPI.LeaveRequests;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace LeaveDayAPI.StoreProcedureProvider
{
    public class StoreProcedureProviderService : LeaveDayAPIAppService, IStoreProcedureProviderService
    {
        private readonly IRepository<LeaveRequest, Guid> _leaveRequestRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        public StoreProcedureProviderService(
            IRepository<LeaveRequest, Guid> leaveRequestRepository,
            IRepository<IdentityUser, Guid> userRepository
        )
        {
            this._leaveRequestRepository = leaveRequestRepository;
            this._userRepository = userRepository;
        }
        public async Task<List<LeaveRequestItemDto>> SearchProcedure(SearchLeaveRequestDto input)
        {
            var result_list = new List<LeaveRequestItemDto>();

            using (var dbContext = _leaveRequestRepository.GetDbContext())
            {
                var connection = dbContext.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "LEAVE_REQUEST_SEARCH";
                    command.CommandType = CommandType.StoredProcedure;

                    var surNameParam = new SqlParameter("@p_surname", input.Surname);
                    command.Parameters.Add(surNameParam);

                    var emailNameParam = new SqlParameter("@p_email", input.Email);
                    command.Parameters.Add(emailNameParam);

                    var startDateNameParam = new SqlParameter("@p_start_date", input.StartDate);
                    command.Parameters.Add(startDateNameParam);

                    var endDateNameParam = new SqlParameter("@p_end_date", input.EndDate);
                    command.Parameters.Add(endDateNameParam);

                    var approveStatusParam = new SqlParameter("@p_approve_status", input.EndDate);
                    command.Parameters.Add(approveStatusParam);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetGuid(reader.GetOrdinal("Id"));
                            var user = await _userRepository.SingleOrDefaultAsync(u => u.Id == id);
                            var leave_request = new LeaveRequestItemDto
                            {
                                Id = id,
                                SurName = user.Surname,
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                CreationTime = reader.GetDateTime(reader.GetOrdinal("CreationTime"))
                            };

                            result_list.Add(leave_request);
                        }
                    }
                }
            }

            return result_list;
        }
    }
}
