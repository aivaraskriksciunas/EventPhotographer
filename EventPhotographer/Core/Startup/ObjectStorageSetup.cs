using Amazon.Runtime;
using Amazon.S3;
using EventPhotographer.Core.Configuration;
using Microsoft.Extensions.Options;

namespace EventPhotographer.Core.Startup;

public static class ObjectStorageSetup
{
    public static void AddObjectStorage(this IServiceCollection services, ObjectStorageConfiguration config)
    {
        var credentials = new BasicAWSCredentials(config.AccessKey, config.Password);
        var s3config = new AmazonS3Config
        {
            ServiceURL = config.ServiceURL,
            ForcePathStyle = config.ForcePathStyle,
        };

        var s3Client = new AmazonS3Client(credentials, s3config);

        services.AddSingleton<IAmazonS3>(s3Client);
    }

    public async static Task SetupObjectStorageAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<ObjectStorageConfiguration>>();
            await s3Client.EnsureBucketExistsAsync(options.Value.BucketName);
        }
    }
}
