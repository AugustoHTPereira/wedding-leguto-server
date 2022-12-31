using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Wedding.Server.API.Options;

namespace Wedding.Server.API.Services;

public interface IStorageService
{
    Task<string> StoreAsync(string key, IFormFile file, string bucketName);
    string GetUrlAsync(string fileName, string bucketName);
}

public class S3StorageService : IStorageService
{
    private readonly StorageOptions _options;
    private readonly AmazonS3Client _client;

    public S3StorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
        _client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, Amazon.RegionEndpoint.USEast1);
    }

    public string GetUrlAsync(string fileName, string bucketName)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = fileName,
            Expires = DateTime.Now.AddHours(1)
        };

        var signedUrl = _client.GetPreSignedURL(request);
        return signedUrl;
    }

    public async Task<string> StoreAsync(string key, IFormFile file, string bucketName)
    {
        var fileStream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = file.ContentType,
            // CannedACL = S3CannedACL.PublicRead,
        };

        var response = await _client.PutObjectAsync(request);
        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return response.ETag;
        }

        return string.Empty;
    }
}