using Amazon.Runtime;
using Amazon.S3;
using EventPhotographer.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventPhotographer.Core.Startup;

public static class ObjectStorageSetup
{
    public static void AddObjectStorage(this IServiceCollection services, ObjectStorageConfiguration config)
    {
        BasicAWSCredentials credentials = new BasicAWSCredentials(config.AccessKey, config.Password);
        AmazonS3Config s3config = new AmazonS3Config
        {
            ServiceURL = config.ServiceURL,
            ForcePathStyle = config.ForcePathStyle,
        };

        AmazonS3Client s3Client = new AmazonS3Client(credentials, s3config);

        services.AddSingleton<IAmazonS3>(s3Client);
    }
}
