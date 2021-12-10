using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Exeptions
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public override string Message { get; }

        public HttpException(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
