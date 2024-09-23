using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;

namespace ProjectManagementAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCodeByExceptionType(exception);
            var result = JsonSerializer.Serialize(new { message = exception.Message });

            Console.WriteLine($"Erro: {exception.Message}, Tipo: {exception.GetType()}, StatusCode: {statusCode}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(result);
        }

        private int GetStatusCodeByExceptionType(Exception exception) => exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,          // 400
            InvalidOperationException => (int)HttpStatusCode.BadRequest,      // 400
            KeyNotFoundException => (int)HttpStatusCode.NotFound,             // 404
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,  // 401
            _ => (int)HttpStatusCode.InternalServerError,                    // 500 - Para exceções não tratadas
        };
    }
}
