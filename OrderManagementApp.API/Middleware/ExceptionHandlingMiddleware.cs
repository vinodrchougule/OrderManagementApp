using FluentValidation;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.DAL.Repositories;
using System.Text.Json;


namespace OrderManagementApp.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = requestDelegate;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                if (ex.Errors != null && ex.Errors.Any())
                {
                    var errors = ex.Errors
                                    .GroupBy(e => e.PropertyName)
                                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage)).ToArray();

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        status = 400,
                        message = "Validation failed",
                        errors
                    }));
                }
                else
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        status = 400,
                        message = ex.Message
                    }));
                }
            }
            catch(NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = 404,
                    message = ex.Message
                }));
            }
            catch (ConcurrencyException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = 409,
                    message = ex.Message
                }));
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                _logger.LogError(ex.Message);

                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = 500,
                    message = ex.Message
                }));
            }
        }
    }
}
