using Provis.Core.Entities.StatusEntity;
using Provis.Core.Entities.StatusHistoryEntity;
using Provis.Core.Entities.WorkspaceTaskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.UnitTests.Base.TestData
{
    public class TaskTestData
    {
        public static List<Status> GetTaskStatusesList()
        {
            return new List<Status>()
            {
                new Status()
                {
                   Id = 1,
                   Name = "In review"
                }
                //},
                //new Status()
                //{
                //   Id = 2,
                //   Name = "Completed"
                //},
                //new Status()
                //{
                //   Id = 3,
                //   Name = "Deleted"
                //}
            };
        }
    }
}
