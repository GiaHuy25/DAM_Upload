using DAM_Upload.Models;
using DAM_Upload.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAM_Upload.db;

namespace DAM_Upload.Services
{
    public class SearchService
    {
        private readonly AppdbContext _context;
        private const int DefaultPageSize = 5;

        public SearchService(AppdbContext context)
        {
            _context = context;
        }

        public async Task<(List<FolderFileDTO> Items, bool HasMore)> SearchAsync(int userId, SearchCriteriaDTO criteria, int skip = 0, int take = DefaultPageSize)
        {
            // Đảm bảo skip và take hợp lệ
            skip = Math.Max(0, skip);
            take = Math.Max(1, take);

            // Lấy danh sách Folders
            IQueryable<Folder> folderQuery = _context.Folders
                .Where(f => f.UserId == userId); // Lọc theo UserId

            // Lấy danh sách Files
            IQueryable<DAM_Upload.Models.File> fileQuery = _context.Files
                .Where(f => f.Folder != null && f.Folder.UserId == userId); // Lọc theo UserId của Folder

            // Áp dụng các tiêu chí tìm kiếm
            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                var nameLower = criteria.Name.ToLower();
                folderQuery = folderQuery.Where(f => f.Name.ToLower().Contains(nameLower));
                fileQuery = fileQuery.Where(f => f.Name.ToLower().Contains(nameLower));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Type))
            {
                if (criteria.Type.ToLower() == "folder")
                {
                    fileQuery = fileQuery.Where(f => false); // Không lấy file
                }
                else if (criteria.Type.ToLower() == "file")
                {
                    folderQuery = folderQuery.Where(f => false); // Không lấy folder
                }
            }

            if (criteria.CreatedFrom.HasValue)
            {
                folderQuery = folderQuery.Where(f => f.CreatedAt >= criteria.CreatedFrom.Value);
                fileQuery = fileQuery.Where(f => f.CreatedAt >= criteria.CreatedFrom.Value);
            }

            if (criteria.CreatedTo.HasValue)
            {
                folderQuery = folderQuery.Where(f => f.CreatedAt <= criteria.CreatedTo.Value);
                fileQuery = fileQuery.Where(f => f.CreatedAt <= criteria.CreatedTo.Value);
            }
            if (!string.IsNullOrWhiteSpace(criteria.Format))
            {
                var formatLower = criteria.Format.ToLower();
                fileQuery = fileQuery.Where(f => f.Format.ToLower() == formatLower);
            }

            // Chuyển đổi Folders thành FolderFileDto
            var folders = await folderQuery
                .Select(f => new FolderFileDTO
                {
                    Id = f.FolderId,
                    Type = "Folder",
                    Name = f.Name,
                    Path = f.Path,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    Size = null,
                    Format = null
                })
                .ToListAsync();

            // Chuyển đổi Files thành FolderFileDto
            var files = await fileQuery
                .Select(f => new FolderFileDTO
                {
                    Id = f.FileId,
                    Type = "File",
                    Name = f.Name,
                    Path = f.Path,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    Size = f.Size,
                    Format = f.Format
                })
                .ToListAsync();

            // Kết hợp danh sách Folders và Files, sắp xếp theo CreatedAt (mới nhất trước)
            var combinedList = folders.Concat(files)
                .OrderByDescending(item => item.CreatedAt)
                .ToList();

            // Tính toán số lượng mục còn lại
            int totalCount = combinedList.Count;
            bool hasMore = skip + take < totalCount;

            // Lấy dữ liệu cho lần tải hiện tại
            var items = combinedList
                .Skip(skip)
                .Take(take)
                .ToList();

            return (items, hasMore);
        }
    }
}