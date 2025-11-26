using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs;
using ProjectManagementService.Application.Features.Projects.Commands;
using ProjectManagementService.Application.Features.Projects.Queries;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.API.Controllers
{
    /// <summary>
    /// Controller quản lý Projects (CRUD + Pagination)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách tất cả projects có phân trang
        /// </summary>
        /// <param name="pageNumber">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số items mỗi trang (mặc định: 10)</param>
        /// <returns>Danh sách projects kèm metadata (TotalPages, TotalCount...)</returns>
        /// <response code="200">Trả về danh sách projects</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<ProjectDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllProjectsQuery(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin project theo ID
        /// </summary>
        /// <param name="id">ID của project</param>
        /// <returns>Thông tin chi tiết project</returns>
        /// <response code="200">Trả về thông tin project</response>
        /// <response code="404">Không tìm thấy project</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _mediator.Send(new GetProjectByIdQuery(id));

            if (result == null)
                throw new NotFoundException($"Không tìm thấy project với id {id}");

            return Ok(ApiResponse<ProjectDto>.Ok(result, "Lấy dữ liệu thành công"));
        }

        /// <summary>
        /// Tạo project mới
        /// </summary>
        /// <param name="command">Thông tin project cần tạo</param>
        /// <returns>ID của project vừa tạo</returns>
        /// <response code="201">Tạo project thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
        {
            var createdId = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(GetById),
                new { id = createdId },
                ApiResponse<long>.Ok(createdId, "Tạo project thành công"));
        }

        /// <summary>
        /// Cập nhật thông tin project
        /// </summary>
        /// <param name="id">ID của project cần cập nhật</param>
        /// <param name="command">Thông tin mới</param>
        /// <returns>Kết quả cập nhật</returns>
        /// <response code="200">Cập nhật thành công</response>
        /// <response code="400">ID không khớp</response>
        /// <response code="404">Không tìm thấy project</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateProjectCommand command)
        {
            if (id != command.Id)
                throw new BusinessRuleException("Id không khớp");

            var result = await _mediator.Send(command);

            if (!result)
                throw new NotFoundException($"Không tìm thấy project với id {id}");

            return Ok(ApiResponse.Ok("Cập nhật thành công"));
        }

        /// <summary>
        /// Xóa project
        /// </summary>
        /// <param name="id">ID của project cần xóa</param>
        /// <returns>Kết quả xóa</returns>
        /// <response code="200">Xóa thành công</response>
        /// <response code="404">Không tìm thấy project</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _mediator.Send(new DeleteProjectCommand(id));

            if (!result)
                throw new NotFoundException($"Không tìm thấy project với id {id}");

            return Ok(ApiResponse.Ok("Xóa thành công"));
        }
    }
}
