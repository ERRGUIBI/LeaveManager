using LeaveManager.Application.DTOs;
using LeaveManager.Domain.Entities;

namespace LeaveManager.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDto>> GetAllAsync();
        Task<LeaveRequestDto> GetByIdAsync(int id);
        Task<int> CreateAsync(LeaveRequestDto dto);
        Task UpdateAsync(LeaveRequestDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<LeaveRequestDto>> FilterLeaveRequestsAsync(LeaveRequestFilterDto filter);
        public List<LeaveRequestSummaryDto> GetLeaveReport(int year);
        public LeaveRequest ApproveLeaveRequest(int id);
    }
}
