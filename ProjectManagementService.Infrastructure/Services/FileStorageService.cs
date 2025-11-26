using Microsoft.Extensions.Logging;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// File storage service implementation
/// TODO: Replace with AWS S3, Azure Blob Storage, or other cloud storage
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _storagePath;

    public FileStorageService(ILogger<FileStorageService> logger)
    {
        _logger = logger;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var fileKey = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(_storagePath, fileKey);

        using (var fileStreamOutput = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }

        _logger.LogInformation("File uploaded: {FileKey}", fileKey);
        return fileKey;
    }

    public async Task<Stream> DownloadFileAsync(string fileKey)
    {
        var filePath = Path.Combine(_storagePath, fileKey);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {fileKey}");
        }

        var memoryStream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open))
        {
            await fileStream.CopyToAsync(memoryStream);
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public Task<bool> DeleteFileAsync(string fileKey)
    {
        var filePath = Path.Combine(_storagePath, fileKey);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            _logger.LogInformation("File deleted: {FileKey}", fileKey);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<string> GetFileUrlAsync(string fileKey, int expirationMinutes = 60)
    {
        // TODO: Generate signed URL for cloud storage
        // For now, return a simple URL
        var url = $"/api/files/{fileKey}";
        return Task.FromResult(url);
    }

    public Task<bool> FileExistsAsync(string fileKey)
    {
        var filePath = Path.Combine(_storagePath, fileKey);
        return Task.FromResult(File.Exists(filePath));
    }
}
