using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Infrastructure.Services;

public class S3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3StorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _bucketName = config["AWS:BucketName"]!;
    }
    [RequestSizeLimit(500 * 1024 * 1024)] // 500MB

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File rỗng");

        // Lấy extension
        var ext = Path.GetExtension(file.FileName);
        var key = $"uploads/{Guid.NewGuid()}{ext}";

        using var stream = file.OpenReadStream();

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(putRequest);

        return "https://exe201-s3.s3.ap-southeast-2.amazonaws.com/"+key;
    }
}