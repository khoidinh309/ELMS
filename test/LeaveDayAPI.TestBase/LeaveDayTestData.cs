using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace LeaveDayAPI
{
    public class LeaveDayTestData : ISingletonDependency
    {
        /*  Users  */
        public Guid ManagerId { get; } = Guid.NewGuid();
        public string ManagerUserName { get; } = "thuanld";

        public Guid UserKhoiId { get; } = Guid.NewGuid();
        public string UserKhoiUserName { get; } = "khoid";

        /* Leave Request */

        public Guid RequestDatingId { get; } = Guid.NewGuid();
        public string RequestDatingTitle { get; } = "Dating";
        public string RequestDatingReason { get; } = "2 year anniversary";

        public DateTime InValidStartDate_Before_Now { get; } = DateTime.Now.AddDays(-1);
        public DateTime InvalidEndDate_Before_Now { get; } = DateTime.Now.AddDays(-2);

        public DateTime ValidStartDate { get; } = DateTime.Now.AddDays(2);
        public DateTime ValidEndDate { get; } = DateTime.Now.AddDays(4);
        public int Number_of_Valid_Days { get; } = 3;

        public DateTime InvaidStartDate_After_End { get; } = DateTime.Now.AddDays(4);
        public DateTime InvaidEndDate_After_End { get; } = DateTime.Now.AddDays(2);

        public Guid RequestWeddingId { get; } = Guid.NewGuid();
        public string RequestWeddingTitle { get; } = "Ex wedding";
        public string RequestWeddingReason { get; } = "I hope that she always be happy";

        public int RemainingDays { get; } = 8;
    }
}
