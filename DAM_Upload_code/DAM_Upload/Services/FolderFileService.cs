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
    public class FolderFileService
    {
        private readonly AppdbContext _context;
        private const int DefaultPageSize = 5;

        public FolderFileService(AppdbContext context)
        {
            _context = context;
        }

        public async Task<(List<FolderFileDTO> Items, bool HasMore)> GetFolderAndFileAsync(int userId, int? folderId = null, int skip = 0, int take = DefaultPageSize)
        {
            // Đảm bảo skip và take hợp lệ
            skip = Math.Max(0, skip);
            take = Math.Max(1, take);

            // Lấy danh sách Folders
            IQueryable<Folder> folderQuery = _context.Folders
                .Where(f => f.UserId == userId); // Lọc theo UserId

            if (folderId.HasValue)
            {
                folderQuery = folderQuery.Where(f => f.ParentId == folderId.Value);
            }

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

            // Lấy danh sách Files
            IQueryable<DAM_Upload.Models.File> fileQuery = _context.Files
                .Where(f => f.Folder != null && f.Folder.UserId == userId); // Lọc theo UserId của Folder

            if (folderId.HasValue)
            {
                fileQuery = fileQuery.Where(f => f.Folder != null && f.Folder.FolderId == folderId.Value);
            }

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