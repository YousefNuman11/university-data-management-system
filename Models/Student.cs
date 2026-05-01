namespace University_System.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int DepartmentId { get; set; }
    }
}