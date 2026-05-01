namespace University_System.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int DepartmentId { get; set; }
    }
}