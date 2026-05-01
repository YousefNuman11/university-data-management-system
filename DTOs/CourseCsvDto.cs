namespace University_System.DTOs
{
    public record CourseCsvDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int DepartmentId { get; set; }
    }
}