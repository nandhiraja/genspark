using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusBooking.Backend.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "Unauthorized access.");
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await WriteErrorResponse(context, HttpStatusCode.Forbidden, "Forbidden access.");
                }
            }
            catch (ArgumentException ex) 
            {
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. " + ex.Message);
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                var response = new { message = message, statusCode = (int)statusCode };
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
