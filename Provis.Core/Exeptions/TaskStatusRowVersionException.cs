using Provis.Core.Resources;
using System.Net;

namespace Provis.Core.Exeptions
{
    public class TaskStatusRowVersionException : HttpException
    {
        public byte[] RowVersion { get; set; }
        public int StatusId { get; set; }

        public TaskStatusRowVersionException(int statusId, byte[] rowVersion) 
            : base(HttpStatusCode.Conflict, ErrorMessages.ConcurrencyCheck)
        {
            RowVersion = rowVersion;
            StatusId = statusId;
        }
    }
}
