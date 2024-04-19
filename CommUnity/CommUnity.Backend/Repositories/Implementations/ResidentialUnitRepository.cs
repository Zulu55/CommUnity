﻿using CommUnity.BackEnd.Data;
using CommUnity.BackEnd.Helpers;
using CommUnity.BackEnd.Repositories.Interfaces;
using CommUnity.Shared.DTOs;
using CommUnity.Shared.Entities;
using CommUnity.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CommUnity.BackEnd.Repositories.Implementations
{
    public class ResidentialUnitRepository : GenericRepository<ResidentialUnit>, IResidentialUnitRepository
    {
        private readonly DataContext _context;

        public ResidentialUnitRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<ResidentialUnit>> GetAsync(int id)
        {
            var residentialUnit = await _context.ResidentialUnits
                .FirstOrDefaultAsync(s => s.Id == id);

            if (residentialUnit == null)
            {
                return new ActionResponse<ResidentialUnit>
                {
                    WasSuccess = false,
                    Message = "Estado no existe"
                };
            }

            return new ActionResponse<ResidentialUnit>
            {
                WasSuccess = true,
                Result = residentialUnit
            };
        }

        public override async Task<ActionResponse<IEnumerable<ResidentialUnit>>> GetAsync()
        {
            var residentialUnit = await _context.ResidentialUnits
                .OrderBy(x => x.Name)
                .ToListAsync();
            return new ActionResponse<IEnumerable<ResidentialUnit>>
            {
                WasSuccess = true,
                Result = residentialUnit
            };
        }

        public override async Task<ActionResponse<IEnumerable<ResidentialUnit>>> GetAsync(PaginationDTO pagination)
        {
            //var queryable = _context.ResidentialUnits
            //    .Where(x => x.City!.Id == pagination.Id)
            //    .AsQueryable();

            var queryable = _context.ResidentialUnits.AsQueryable();

            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.City!.Id == pagination.Id);
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<ResidentialUnit>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.ResidentialUnits
                .Where(x => x.City!.Id == pagination.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

    }
}