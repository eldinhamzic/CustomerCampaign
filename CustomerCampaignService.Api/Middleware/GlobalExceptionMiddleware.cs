using CustomerCampaignService.Application.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CustomerCampaignService.Api.Middleware;

public static class GlobalExceptionMiddleware
{
    public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";

                var exception = context.Features
                    .Get<IExceptionHandlerFeature>()?.Error;

                var problem = exception is CCSException ccs
                    ? new ProblemDetails
                    {
                        Status = ccs.StatusCode,
                        Title = ccs.Title,
                        Detail = ccs.Message,
                        Instance = context.Request.Path
                    }
                    : new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Unexpected error",
                        Detail = "Došlo je do greške. Pokušaj kasnije.",
                        Instance = context.Request.Path
                    };

                problem.Extensions["traceId"] = context.TraceIdentifier;

                context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(problem);
            });
        });
    }
}
