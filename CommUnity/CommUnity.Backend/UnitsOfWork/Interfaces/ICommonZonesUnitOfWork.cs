﻿using CommUnity.Shared.DTOs;
using CommUnity.Shared.Entities;
using CommUnity.Shared.Responses;

namespace CommUnity.BackEnd.UnitsOfWork.Interfaces
{
    public interface ICommonZonesUnitOfWork
    {
        Task<ActionResponse<CommonZone>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<CommonZone>>> GetAsync();

        Task<ActionResponse<IEnumerable<CommonZone>>> GetAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<ActionResponse<int>> GetRecordsNumber(PaginationDTO pagination);
    }
}