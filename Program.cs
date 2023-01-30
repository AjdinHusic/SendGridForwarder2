using DotNetEnv;
using SendGridForwarder2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var a = Env.Load(Directory.GetCurrentDirectory());
Env.Load();
Env.TraversePath().Load();

var sendGridApiKey = Env.GetString("SENDGRID_API_KEY");


if (string.IsNullOrWhiteSpace(sendGridApiKey)) throw new Exception("Missing environment variable: SENDGRID_API_KEY");

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

app.UseAuthorization();

app.MapControllers();

app.Run();