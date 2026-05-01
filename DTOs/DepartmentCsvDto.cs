namespace University_System.DTOs
{
    public record DepartmentCsvDto
    {
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTime EstablishmentDate { get; set; }
    }
}