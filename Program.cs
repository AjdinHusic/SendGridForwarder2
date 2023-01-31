using System.Collections;
using System.Threading.RateLimiting;
using DotNetEnv;
using Microsoft.Extensions.Options;
using SendGridForwarder2;
using SendGridForwarder2.Extensions;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
Env.TraversePath().Load();

var sendGridApiKey = Env.GetString("SENDGRID_API_KEY");

var variables = Environment
    .GetEnvironmentVariables()
    .OfType<DictionaryEntry>()
    .Where(entry => entry.Key is string s && s.Contains("MAIL_PRESET_"))
    .Where(entry => entry.Value is string)
    .ToDictionary(entry => (string)entry.Key, entry => (string)entry.Value);;

if (string.IsNullOrWhiteSpace(sendGridApiKey)) throw new Exception("Missing environment variable: SENDGRID_API_KEY");

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = Env.GetInt("RATE_LIMIT", 5),
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});
builder.Services.AddScoped<IEmailService, EmailService>(x => new EmailService(sendGridApiKey));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthorization();

app.DisableEmailRouteIfDisallowed();
app.MapEmailPresetsToEndpoints(variables);
app.MapControllers();


app.Run();