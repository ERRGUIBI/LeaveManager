namespace LeaveManager.Application.DTOs
{
    public class LeaveRequestSummaryDto
    {
        public string EmployeeName { get; set; }
        public int TotalLeaves { get; set; }
        public int AnnualLeaves { get; set; }
        public int SickLeaves { get; set; }
    }
}
