namespace ProjectManagementService.Application.Common;

// Response cho danh sách có phân trang
public class PaginatedResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public List<T> Data { get; set; } = new();
    public int PageNumber { get; set; }  // Trang hiện tại
    public int PageSize { get; set; }     // Số item mỗi trang
    public int TotalPages { get; set; }   // Tổng số trang
    public int TotalRecords { get; set; } // Tổng số bản ghi

    public static PaginatedResponse<T> Create(List<T> data, int page, int pageSize, int totalRecords)
    {
        return new PaginatedResponse<T>
        {
            Success = true,
            Message = "Lấy dữ liệu thành công",
            Data = data,
            PageNumber = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }
}
