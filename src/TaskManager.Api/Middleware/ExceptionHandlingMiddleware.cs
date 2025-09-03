using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;

namespace TaskManager.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _problemFactory;

    public ExceptionHandlingMiddleware(RequestDelegate next, ProblemDetailsFactory problemFactory)
    {
        _next = next;
        _problemFactory = problemFactory;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (ValidationException vex)
        {
            // Convert FluentValidation errors => ValidationProblemDetails (400)
            var modelState = new ModelStateDictionary();
            foreach (var error in vex.Errors)
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);

            var pd = _problemFactory.CreateValidationProblemDetails(
                ctx, modelState,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation failed");

            Log.Warning("Validation failed {Path} {@Errors}", ctx.Request.Path, vex.Errors);

            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(pd);
        }
        catch (Exception ex)
        {
            // Generic 500 with ProblemDetails (no internal details leaked)
            var pd = _problemFactory.CreateProblemDetails(
                ctx,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                detail: "Contact support with the correlation id.",
                instance: ctx.TraceIdentifier);

            Log.Error(ex, "Unhandled exception {Path} TraceId={TraceId}", ctx.Request.Path, ctx.TraceIdentifier);

            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsJsonAsync(pd);
        }
    }
}
