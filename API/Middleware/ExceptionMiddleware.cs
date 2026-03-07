using System;
using System.Net;
using System.Text.Json;
using Application.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware;

public class ExceptionMiddleware(IHostEnvironment env, ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = env.IsDevelopment()
            ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) //if development
            : new AppException(context.Response.StatusCode, "Internal Server Error", null);//not development == PROD

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json);
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        var validationErrors = new Dictionary<string, string[]>();

        if (ex.Errors is not null)
        {
            foreach (var error in ex.Errors)
            {
                if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                {
                    /*
                    The Spread Operator (..): This "unpacks" or "spreads" the contents of existingErrors into the new collection. 
                                        It's like saying, "Take everything that was already in the list and put it here."

                    The Comma & New Item: After the spread, you add error.ErrorMessage to the end of that new sequence.

                    The Brackets []: These wrap the whole thing into a new collection expression (in this case, an array of strings).
                    */

                    validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                    /*
                    // The old way (pre-C# 12)
                    var newList = new List<string>(existingErrors);
                    newList.Add(error.ErrorMessage);
                    validationErrors[error.PropertyName] = newList.ToArray();
                    */
                }
                else
                {
                    validationErrors[error.PropertyName] = [error.ErrorMessage];
                }
            }
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var validationProblemDetails = new ValidationProblemDetails(validationErrors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "ValidationFailure",
            Title = "Validation error",
            Detail = "One or more validation errors has occurred"
        };

        await context.Response.WriteAsJsonAsync(validationProblemDetails);
    }
}