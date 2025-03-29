using DAM_Upload.Dtos;
using DAM_Upload.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DAM_Upload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly FolderFileService _folderFileService;
        private readonly Authservice _authService;
        private readonly SearchService _searchService;
        public FolderController(FolderFileService folderFileService, Authservice authService, SearchService searchService)
        {
            _folderFileService = folderFileService;
            _authService = authService;
            _searchService = searchService;
        }
        [HttpGet]
        public async Task<IActionResult> GetFolderAndFile(int? folderId = null, int skip = 0)
        {
            try
            {
                // Lấy UserId từ JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }

                // Gọi service để lấy danh sách Folder và File
                var (items, hasMore) = await _folderFileService.GetFolderAndFileAsync(userId, folderId, skip);
                return Ok(new
                {
                    Items = items,
                    HasMore = hasMore,
                    Skip = skip,
                    Take = 5
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCriteriaDTO criteria = null, int skip = 0)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user.");
                }

                criteria ??= new SearchCriteriaDTO();

                var (items, hasMore) = await _searchService.SearchAsync(userId, criteria, skip);
                return Ok(new
                {
                    Items = items,
                    HasMore = hasMore,
                    Skip = skip,
                    Take = 5
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}