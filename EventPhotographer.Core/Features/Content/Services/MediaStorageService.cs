using Amazon.S3;
using Amazon.S3.Model;

namespace EventPhotographer.Core.Features.Content.Services;

public class MediaStorageService(
    IAmazonS3 s3Client)
{
    private const string BucketName = "event-photographer";

    public async Task<string> UploadFile(Stream fileStream, string contentType, string key)
    {
        await s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
        });

        return key;
    }

    public async Task<GetObjectResponse> GetFileAsync(string key)
    {
        return await s3Client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = BucketName,
            Key = key,
        });
    }
}
