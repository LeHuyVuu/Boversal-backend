using Amazon.S3;
using Amazon.S3.Model;

namespace UtilityService.Infrastructure.Services;

public class S3StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;

    public S3StorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _bucketName = config["AWS:BucketName"] ?? throw new ArgumentNullException("AWS:BucketName is required");
        _region = config["AWS:Region"] ?? "ap-southeast-2";
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // Validate file size (500MB max)
        if (file.Length > 500 * 1024 * 1024)
            throw new ArgumentException("File size exceeds 500MB limit");

        // Get extension and generate unique key
        var ext = Path.GetExtension(file.FileName);
        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var sanitizedFileName = string.Join("", fileName.Split(Path.GetInvalidFileNameChars()));
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var key = $"uploads/{timestamp}_{sanitizedFileName}_{Guid.NewGuid():N}{ext}";

        using var stream = file.OpenReadStream();

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType,
            CannedACL = S3CannedACL.PublicRead // Make file publicly accessible
        };

        await _s3Client.PutObjectAsync(putRequest);

        // Return S3 URL
        return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";
    }
}