using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Provis.Core.Exeptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Provis.WebApi.Middleweres
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";



                if (ex is HttpException httpException)
                {
                    context.Response.StatusCode = (int)httpException.StatusCode;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = httpException.Message }));
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = "Unknown error has occured" }));
                return;
            }
        }
    }
}
