using Amazon.S3;
using Amazon.S3.Model;

namespace EventPhotographer.App.Content.Services;

public class MediaStorageService(
    IAmazonS3 s3Client)
{
    private const string BucketName = "event-photographer";

    public async Task<string> UploadFile(IFormFile file, string key)
    {
        using (var fileStream = file.OpenReadStream())
        {
            await s3Client.PutObjectAsync(new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = BucketName,
                Key = key,
                InputStream = fileStream,
                ContentType = file.ContentType,
            });
        }

        return key;
    }

    public async Task<GetObjectResponse> GetFileAsync(string key)
    {
        return await s3Client.GetObjectAsync(new Amazon.S3.Model.GetObjectRequest
        {
            BucketName = BucketName,
            Key = key,
        });
    }
}
