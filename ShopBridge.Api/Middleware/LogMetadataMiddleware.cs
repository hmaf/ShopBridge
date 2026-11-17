using Microsoft.AspNetCore.Mvc.Controllers;
using ShopBridge.Application.Abstractions.Logging;
using System;

namespace ShopBridge.Api.Middleware;

public class LogMetadataMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ILogMetadataResolver resolver)
    {
        var endpoint = context.GetEndpoint();

        var descriptor = endpoint?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        if (descriptor is not null)
        {
            var actionId = await resolver.GetActionIdAsync(
                descriptor.ControllerName,
                descriptor.ActionName,
                context.RequestAborted);

            context.Items["ActionId"] = actionId;

            using (Serilog.Context.LogContext.PushProperty(
                "ActionId", actionId))
            {
                await next(context);
            }

            return;
        }

        await next(context);
    }
}