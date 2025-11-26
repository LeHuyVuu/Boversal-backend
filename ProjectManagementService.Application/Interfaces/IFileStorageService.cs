namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// File storage service interface - implement in Infrastructure layer
/// Example: AWS S3, Azure Blob Storage, Local file system
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task<bool> DeleteFileAsync(string fileKey);
    Task<string> GetFileUrlAsync(string fileKey, int expirationMinutes = 60);
    Task<bool> FileExistsAsync(string fileKey);
}
