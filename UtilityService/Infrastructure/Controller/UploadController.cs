using UtilityService.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using UtilityService.Models;

namespace UtilityService.Infrastructure.Controller;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly S3StorageService _s3Service;

    public UploadController(S3StorageService s3Service)
    {
        _s3Service = s3Service;
    }

    /// <summary>
    /// Upload file to S3
    /// </summary>
    /// <param name="file">File to upload (max 500MB)</param>
    /// <returns>ApiResponse with file URL</returns>
    [HttpPost("upload")]
    [RequestSizeLimit(500 * 1024 * 1024)] // 500MB
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Fail(400, "No file uploaded"));

        try
        {
            var url = await _s3Service.UploadFileAsync(file);
            return Ok(ApiResponse<string>.Success(url, "File uploaded successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(500, "Upload failed", ex.Message));
        }
    }

    /// <summary>
    /// Upload multiple files to S3
    /// </summary>
    /// <param name="files">Files to upload</param>
    /// <returns>ApiResponse with list of file URLs</returns>
    [HttpPost("upload-multiple")]
    [RequestSizeLimit(500 * 1024 * 1024)]
    public async Task<IActionResult> UploadMultiple(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(ApiResponse<List<string>>.Fail(400, "No files uploaded"));

        try
        {
            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var url = await _s3Service.UploadFileAsync(file);
                    urls.Add(url);
                }
            }

            return Ok(ApiResponse<List<string>>.Success(urls, $"{urls.Count} files uploaded successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<string>>.Fail(500, "Upload failed", ex.Message));
        }
    }
}