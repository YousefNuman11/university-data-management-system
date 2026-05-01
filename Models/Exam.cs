namespace University_System.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int TotalMarks { get; set; }
        public int CourseId { get; set; }
    }
}