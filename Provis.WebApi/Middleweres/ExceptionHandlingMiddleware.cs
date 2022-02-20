using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Provis.Core.Exeptions;
using Provis.Core.Exeptions.FileExceptions;
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
            catch(FileException ex)
            {
                HttpStatusCode statusCode = (ex is FileNotFoundException) ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
                await CreateErrorAsync(context, statusCode, new { error = ex.Message });
                return;
            }
            catch(TaskStatusRowVersionException ex)
            {
                await CreateErrorAsync(context, ex.StatusCode, 
                    new 
                    { 
                        error = ex.Message, 
                        statusId = ex.StatusId, 
                        rowVersion = ex.RowVersion 
                    });
                return;
            }
            catch (HttpException ex)
            {
                await CreateErrorAsync(context, ex.StatusCode, new { error = ex.Message });
                return;
            }
            catch(Exception)
            {
                await CreateErrorAsync(context);
                return;
            }
        }

        private async Task CreateErrorAsync(
            HttpContext context,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            object errorBody = null)
        {
            _ = errorBody ?? new { error = "Unknown error has occured" };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorBody));
        }
    }
}
