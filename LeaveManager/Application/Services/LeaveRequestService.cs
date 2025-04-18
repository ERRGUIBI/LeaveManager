using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using LeaveManager.Application.DTOs;
using LeaveManager.Application.Interfaces;
using LeaveManager.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private static List<Employee> _employees = new()
{
    new Employee { Id = 2, FullName = "Ali Ben", Department = "IT", JoiningDate = DateTime.Today.AddYears(-2) },
    new Employee { Id = 3, FullName = "Sana M.", Department = "RH", JoiningDate = DateTime.Today.AddYears(-1) },
    new Employee { Id = 4, FullName = "Sana M.", Department = "RH", JoiningDate = DateTime.Today.AddYears(-1) },
};
        private static List<LeaveRequestDto> _leaveRequests = new()
{
    new LeaveRequestDto
    {
        Id = 1,
        EmployeeId = 3,
        LeaveType = "Annual",
        Status = "Pending",
        StartDate = new DateTime(2024, 04, 10),
        EndDate = new DateTime(2024, 04, 15),
        Reason = "Vacances"
    },
    new LeaveRequestDto
    {
        Id = 2,
        EmployeeId = 3,
        LeaveType = "Sick",
        Status = "Pending",
        StartDate = new DateTime(2024, 03, 01),
        EndDate = new DateTime(2024, 03, 03),
        Reason = "Grippe"
    },
    new LeaveRequestDto
    {
        Id = 3,
        EmployeeId = 2,
        LeaveType = "Annual",
        Status = "Approved",
        StartDate = new DateTime(2024, 01, 10),
        EndDate = new DateTime(2024, 01, 15),
        Reason = "Voyage"
    }
};
        private readonly IMapper _mapper;

        public LeaveRequestService(IMapper mapper)
        {

            _mapper = mapper;
        }

        public Task<IEnumerable<LeaveRequestDto>> GetAllAsync() =>
       Task.FromResult(_leaveRequests.AsEnumerable());

        public Task<LeaveRequestDto> GetByIdAsync(int id) =>
            Task.FromResult(_leaveRequests.FirstOrDefault(lr => lr.Id == id));

        public Task<int> CreateAsync(LeaveRequestDto dto)
        {
            dto.Id = _leaveRequests.Max(lr => lr.Id) + 1;
            _leaveRequests.Add(dto);
            return Task.FromResult(dto.Id);
        }

        public Task UpdateAsync(LeaveRequestDto dto)
        {
            var existing = _leaveRequests.FirstOrDefault(lr => lr.Id == dto.Id);
            if (existing != null)
            {
                existing.EmployeeId = dto.EmployeeId;
                existing.LeaveType = dto.LeaveType;
                existing.Status = dto.Status;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.Reason = dto.Reason;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var existing = _leaveRequests.FirstOrDefault(lr => lr.Id == id);
            if (existing != null)
            {
                _leaveRequests.Remove(existing);
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<LeaveRequestDto>> FilterLeaveRequestsAsync(LeaveRequestFilterDto filter)
        {
            var query = _leaveRequests.AsQueryable();

            if (filter.EmployeeId.HasValue)
                query = query.Where(lr => lr.EmployeeId == filter.EmployeeId.Value);

            if (!string.IsNullOrWhiteSpace(filter.LeaveType))
                query = query.Where(lr => lr.LeaveType == filter.LeaveType);

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(lr => lr.Status == filter.Status);

            if (filter.StartDate.HasValue)
                query = query.Where(lr => lr.StartDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(lr => lr.EndDate <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(lr => lr.Reason != null && lr.Reason.ToLower().Contains(filter.Keyword.ToLower()));

            // ✨ Tri dynamique sans EF.Property
            var sortBy = filter.SortBy ?? "StartDate";
            var sortOrder = filter.SortOrder?.ToLower() ?? "asc";

            var parameter = Expression.Parameter(typeof(LeaveRequestDto), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda<Func<LeaveRequestDto, object>>(Expression.Convert(property, typeof(object)), parameter);

            query = sortOrder == "desc" ? query.OrderByDescending(lambda) : query.OrderBy(lambda);

            // ⏳ Pagination
            int skip = (filter.Page - 1) * filter.PageSize;
            query = query.Skip(skip).Take(filter.PageSize);

            var results = query.ToList();

            // ✅ Règle : pas de Sick sans raison
            foreach (var r in results.Where(x => x.LeaveType == "Sick"))
            {
                if (string.IsNullOrWhiteSpace(r.Reason))
                    throw new Exception($"Sick leave without reason (ID: {r.Id})");
            }

            // ✅ Règle : max 20 jours annuels / an / employé
            var grouped = results
                .Where(lr => lr.LeaveType == "Annual")
                .GroupBy(lr => new { lr.EmployeeId, lr.StartDate.Year });

            foreach (var group in grouped)
            {
                int totalDays = group.Sum(lr => (lr.EndDate - lr.StartDate).Days + 1);
                if (totalDays > 20)
                    throw new Exception($"Employee {group.Key.EmployeeId} dépasse 20 jours de congé annuel en {group.Key.Year}");
            }

            // ✅ Règle : pas de chevauchement de dates
            foreach (var empGroup in results.GroupBy(r => r.EmployeeId))
            {
                var list = empGroup.OrderBy(r => r.StartDate).ToList();
                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i].StartDate <= list[i - 1].EndDate)
                        throw new Exception($"Chevauchement de congé détecté pour l'employé {list[i].EmployeeId}");
                }
            }

            return await Task.FromResult(results);
        }

        public List<LeaveRequestSummaryDto> GetLeaveReport(int year)
        {
            var report = new List<LeaveRequestSummaryDto>();

            
            var filteredLeaveRequests = _leaveRequests.Where(lr => lr.StartDate.Year == year).ToList();

           
            var groupedByEmployee = filteredLeaveRequests
                .GroupBy(lr => lr.EmployeeId)
                .ToList();

            foreach (var group in groupedByEmployee)
            {
                var employee = _employees.FirstOrDefault(e => e.Id == group.Key);
                if (employee != null)
                {
                    var annualLeaves = group.Count(lr => lr.LeaveType == "Annual");
                    var sickLeave = group.Count(lr => lr.LeaveType == "Sick");

                    var totalLeaves = annualLeaves + sickLeave;

                    report.Add(new LeaveRequestSummaryDto
                    {
                        EmployeeName = employee.FullName,
                        TotalLeaves = totalLeaves,
                        AnnualLeaves = annualLeaves,
                        SickLeaves = sickLeave
                    });
                }
            }

            return report;
        }

        public LeaveRequest ApproveLeaveRequest(int id)
        {
            var leaveRequestDto = _leaveRequests.FirstOrDefault(lr => lr.Id == id);

            if (leaveRequestDto == null)
                throw new Exception("Leave request not found.");

            if (leaveRequestDto.Status != "Pending")
                throw new Exception("Only Pending requests can be approved.");

            leaveRequestDto.Status = "Approved";

            // Mapper LeaveRequestDto en LeaveRequest
            var leaveRequestEntity = _mapper.Map<LeaveRequest>(leaveRequestDto);

            return leaveRequestEntity;
        }

    }
}
