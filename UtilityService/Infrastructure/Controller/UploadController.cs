    // using CustomerService.Infrastructure.Services;
    // using Microsoft.AspNetCore.Mvc;
    // using UtilityService.Models;
    //
    // namespace UtilityService.Infrastructure.Controller;
    //
    // [ApiController]
    // [Route("api/[controller]")]
    // public class UploadController : ControllerBase
    // {
    //     private readonly S3StorageService _s3Service;
    //
    //     public UploadController(S3StorageService s3Service)
    //     {
    //         _s3Service = s3Service;
    //     }
    //
    //     [HttpPost("")]
    //     public async Task<IActionResult> Upload(IFormFile file)
    //     {
    //         if (file == null || file.Length == 0)
    //             return BadRequest("No file uploaded.");
    //
    //         var url = await _s3Service.UploadFileAsync(file);
    //
    //         return Ok(ApiResponse<object>.Success(url, "File uploaded successfully"));
    //     }
    // }