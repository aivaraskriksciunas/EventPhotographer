using Amazon.S3;
using Amazon.S3.Model;
using EventPhotographer.Core.Configuration;
using Microsoft.Extensions.Options;

namespace EventPhotographer.Core.Features.Content.Services;

public class MediaStorageService(
    IAmazonS3 s3Client, 
    IOptions<ObjectStorageConfiguration> _options)
{
    private readonly ObjectStorageConfiguration options = _options.Value;

    public async Task<string> UploadFile(Stream fileStream, string contentType, string key)
    {
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = options.BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true,
        });

        return key;
    }

    public async Task<GetObjectResponse> GetFileAsync(string key)
    {
        return await s3Client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = options.BucketName,
            Key = key,
        });
    }
}
