using System.Net;
using DotNetEnv;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SendGridForwarder2.Extensions;

public static class WebApplicationExtensions
{
    public static void MapEmailPresetsToEndpoints(this WebApplication app, Dictionary<string, string>? variables)
    {
        var index = 0;
        while (variables.ContainsKey($"MAIL_PRESET_{index}"))
        {
            var preset = variables[$"MAIL_PRESET_{index}"];
            index++;
            if (string.IsNullOrWhiteSpace(preset)) continue;
            var presetVariables = preset
                .Split(';')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);
            presetVariables.TryGetValue("route", out var route);
            presetVariables.TryGetValue("from", out var from);
            presetVariables.TryGetValue("to", out var to);
            presetVariables.TryGetValue("replyTo", out var replyTo);
            presetVariables.TryGetValue("subject", out var subject);

            if (string.IsNullOrWhiteSpace(route)) continue;

            Console.WriteLine($"added route {route}");
            app.MapPost(route, async (c) =>
            {
                var bodyContent = await c.Request.ReadFromJsonAsync<ForwardEmailRequest>();
                var service = c.RequestServices.GetService<IEmailService>();
                var response = await service.ForwardEmail(new ForwardEmailRequest(
                    from ?? bodyContent.From, 
                    to ?? bodyContent.To, 
                    replyTo ?? bodyContent.ReplyTo, 
                    subject ?? bodyContent.Subject, 
                    bodyContent.Body));
                await c.Response.WriteAsJsonAsync(response);
            });
        }
    }

    public static void DisableEmailRouteIfDisallowed(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (!Env.GetBool("ALLOW_OPEN_EMAIL_ENDPOINT") && context.Request.Path.Value.ToLower().EndsWith("email"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("Not found.");
                return;
            }

            await next(context);
        });
    }
}