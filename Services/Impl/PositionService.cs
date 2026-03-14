using AttendanceManagementApp.Configs;
using AttendanceManagementApp.DTOs.Request;
using AttendanceManagementApp.DTOs.Response;
using AttendanceManagementApp.Exception;
using AttendanceManagementApp.Mappings;
using AttendanceManagementApp.Models;
using AttendanceManagementApp.Repositories;
using AttendanceManagementApp.Services.Interface;
using AttendanceManagementApp.Utils;
using Microsoft.EntityFrameworkCore;

namespace AttendanceManagementApp.Services.Impl
{
    public class PositionService : IPositionService
    {
        private readonly IRepository<Position> _repo;
        private readonly AppDbContext _context;
        private readonly PositionMapping _positionMapping;

        public PositionService(IRepository<Position> repo, AppDbContext context, PositionMapping positionMapping)
        {
            _repo = repo;
            _context = context;
            _positionMapping = positionMapping;
        }

        public async Task<PositionRes> CreatePositionAsync(PositionCreateReq positionCreateReq)
        {
            var position = new Position
            {
                Name = positionCreateReq.Name,
                Description = positionCreateReq.description
            };
            await _repo.AddAsync(position);
            await _repo.SaveAsync();
            return _positionMapping.ToPositionRes(position);
        }

        public async Task<PagedResult<PositionRes>> GetPositionsAsync(PaginationQuery query)
        {
            var queryable = _context.Positions
                .AsNoTracking()
                .ApplySearch(query.Search, x => x.Name)
                .ApplySorting(query.SortBy, query.Desc);

            var total = await queryable.CountAsync();

            var items = await queryable
                .ApplyPagination(query.Page, query.PageSize)
                .ToListAsync();

            var itemsRes = items.Select(x => _positionMapping.ToPositionRes(x)).ToList();

            return new PagedResult<PositionRes>
            {
                Total = total,
                Items = itemsRes,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<PositionRes> SoftDeletePositionAsync(int id)
        {
            var position =  _repo.GetByIdAsync(id).Result;
            if (position == null)
            {
                throw new NotFoundException("Position not found.");
            }
            _repo.SoftDelete(position);
            await _repo.SaveAsync();
            return _positionMapping.ToPositionRes(position);
        }

        public async Task<PositionRes> UpdatePositionAsync(int id, PositionCreateReq positionCreateReq)
        {
            var position = _repo.GetByIdAsync(id).Result;
            if (position == null)
            {
                throw new NotFoundException("Position not found.");
            }
            _repo.Update(position);
            await _repo.SaveAsync();
            return _positionMapping.ToPositionRes(position);
        }
    }
}
