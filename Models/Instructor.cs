namespace University_System.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rank { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
    }
}