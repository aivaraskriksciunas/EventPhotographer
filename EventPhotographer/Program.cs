using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataServices(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ApplicationException("Database DefaultConnection is not provided"));
builder.Services.AddHostedService<DatabaseStartup>();

builder.Services.AddObjectStorage(
    builder.Configuration.GetSection("ObjectStorage").Get<ObjectStorageConfiguration>() ?? throw new ApplicationException("Object storage settings are not configured")
);
builder.Services.AddHostedService<ObjectStorageStartup>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity
builder.Services.AddAppAuth();

// Setup application modules
builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAppModules();
builder.Services.AddAppExceptionHandlers();
builder.Services.ConfigureApplicationCors(builder.Configuration.GetValue<string>("ClientUrl") ?? throw new ArgumentException("ClientUrl is not configured"));

var app = builder.Build();

// Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Production
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseExceptionHandler("/Error");

app.UseRouting();
app.UseApplicationCorsPolicy();
app.UseApplicationMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();

app.Run();

public partial class Program { }