using Amazon.Extensions.NETCore.Setup;
using EventPhotographer.Core;
using EventPhotographer.Core.Configuration;
using EventPhotographer.Core.Startup;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddObjectStorage(
    builder.Configuration.GetSection("ObjectStorage").Get<ObjectStorageConfiguration>() ?? throw new ConfigurationException("Object storage settings are not configured")
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity
builder.Services.AddAppAuth();

// Setup application modules
builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddAppModules();
builder.Services.AddAppExceptionHandlers();
builder.Services.ConfigureApplicationCors();

var app = builder.Build();

// Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();

    app.UseDevelopmentCorsPolicy();
}

// Production
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.SetupObjectStorageAsync();

app.UseExceptionHandler("/Error");
app.UseHttpsRedirection();

app.UseRouting();
app.UseApplicationMiddleware();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();

app.Run();

public partial class Program { }