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
            DisablePayloadSigning = false,
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

    public async Task<CreatePresignedPostResponse> CreatePresignedUrl(
        string identifier, 
        long fileSize)
    {
        return await s3Client.CreatePresignedPostAsync(new CreatePresignedPostRequest
        {
            BucketName = options.BucketName,
            Key = identifier,
            Expires = DateTime.UtcNow.AddMinutes(10),
            Conditions = [
                S3PostCondition.ContentLengthRange(1, fileSize),
            ],
        });
    }
}
