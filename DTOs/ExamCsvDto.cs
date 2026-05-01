namespace University_System.DTOs
{
    public record ExamCsvDto
    {
        public int ExamId { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int TotalMarks { get; set; }
        public int CourseId { get; set; }
    }
}