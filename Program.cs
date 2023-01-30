using System.Threading.RateLimiting;
using DotNetEnv;
using SendGridForwarder2;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
Env.TraversePath().Load();

var sendGridApiKey = Env.GetString("SENDGRID_API_KEY");


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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();