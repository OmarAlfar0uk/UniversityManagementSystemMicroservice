using Auth_Service.Features.Shared;
using FluentValidation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Auth.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled Exception | {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            string message;
            List<string> errors = new();

            switch (ex)
            {
                // ── FluentValidation ────────────────────────────────────────────
                case ValidationException validationEx:
                    statusCode = 400;
                    message = "Validation failed";
                    errors = validationEx.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();
                    break;

                // ── Bad JSON / missing required fields ──────────────────────────
                case BadHttpRequestException badReqEx:
                    statusCode = 400;
                    message = badReqEx.Message;
                    break;

                // ── Not Found ───────────────────────────────────────────────────
                case KeyNotFoundException keyEx:
                    statusCode = 404;
                    message = keyEx.Message;
                    break;

                // ── Unauthorized / Forbidden ────────────────────────────────────
                case UnauthorizedAccessException:
                    statusCode = context.User.Identity?.IsAuthenticated == true ? 403 : 401;
                    message = statusCode == 403
                        ? "You do not have permission to perform this action"
                        : "Authentication required";
                    break;

                // ── EF Core DB errors (FK violations, timeouts, etc.) ───────────
                case DbUpdateException dbEx:
                    statusCode = 500;
                    var sqlEx = dbEx.InnerException as SqlException;
                    message = sqlEx?.Number switch
                    {
                        547  => "Operation blocked: related data exists. Remove dependent records first.",
                        2601 => "A record with this value already exists (duplicate key).",
                        2627 => "A record with this value already exists (unique constraint).",
                        _    => "A database error occurred while saving changes."
                    };
                    if (_env.IsDevelopment())
                        errors.Add(dbEx.InnerException?.Message ?? dbEx.Message);
                    break;

                // ── InvalidOperationException (e.g. completed transaction) ──────
                case InvalidOperationException ioEx:
                    statusCode = 500;
                    message = "An internal operation failed. Please try again.";
                    if (_env.IsDevelopment())
                        errors.Add(ioEx.Message);
                    break;

                // ── Default ─────────────────────────────────────────────────────
                default:
                    statusCode = 500;
                    message = "An unexpected error occurred";
                    if (_env.IsDevelopment())
                        errors.Add(ex.Message);
                    break;
            }

            // Always return the same EndpointResponse shape
            var response = EndpointResponse<object>.ErrorResponse(message, statusCode, errors);

            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
