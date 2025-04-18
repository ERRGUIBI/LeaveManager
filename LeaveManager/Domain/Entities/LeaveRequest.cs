using LeaveManager.Domain.Enum;

namespace LeaveManager.Domain.Entities
{
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public Employee Employee { get; set; }
    }
}
