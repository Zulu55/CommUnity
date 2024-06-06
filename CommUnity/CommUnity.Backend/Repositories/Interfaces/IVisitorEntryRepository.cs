﻿using CommUnity.Shared.DTOs;
using CommUnity.Shared.Entities;
using CommUnity.Shared.Enums;
using CommUnity.Shared.Responses;

namespace CommUnity.BackEnd.Repositories.Interfaces
{
    public interface IVisitorEntryRepository
    {
        Task<ActionResponse<VisitorEntry>> ScheduleVisitor(string email, VisitorEntryDTO visitorEntryDTO);

        Task<ActionResponse<VisitorEntry>> AddVisitor(string email, VisitorEntryDTO visitorEntryDTO);

        Task<ActionResponse<IEnumerable<VisitorEntry>>> GetVisitorEntryByStatus(string email, VisitorStatus status);

        Task<ActionResponse<IEnumerable<VisitorEntry>>> GetVisitorEntryByApartment(string email, int apartmentId);

        Task<ActionResponse<VisitorEntry>> ConfirmVisitorEntry(string email, VisitorEntryDTO visitorEntryDTO);

        Task<ActionResponse<VisitorEntry>> CancelVisitorEntry(string email, VisitorEntryDTO visitorEntryDTO);
    }
}