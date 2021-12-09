using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Provis.Core.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Provis.WebApi.Middlwares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                if(ex is HttpStatusException http)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { error = http.Message}));
                }
            }
        }

    }
}
