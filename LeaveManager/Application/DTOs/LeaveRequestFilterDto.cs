namespace LeaveManager.Application.DTOs
{
    public class LeaveRequestFilterDto
    {
        public int? EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Keyword { get; set; }

        public string? SortBy { get; set; } = "StartDate";
        public string? SortOrder { get; set; } = "asc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
