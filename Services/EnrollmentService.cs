using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class EnrollmentService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public EnrollmentService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var enrollmentDtos = ReadExamFromCvs(filePath);
            Console.WriteLine($"Read {enrollmentDtos.Count} rows from CSV.");

            var enrollments = _mapper.Map<List<Enrollment>>(enrollmentDtos);
            InsertEnrollmentInDatabase(enrollments);

        }

        private List<EnrollmentCsvDto> ReadExamFromCvs(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim()
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<EnrollmentCsvDto>().ToList();
        }

        private void InsertEnrollmentInDatabase(List<Enrollment> enrollments)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            var inserted = 0;

            foreach (var enrollment in enrollments)
            {
                string query = @"
                    INSERT INTO Enrollment (StudentId, CourseId)
                    VALUES (@StudentId, @CourseId)";

                using SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StudentId", enrollment.StudentId);
                cmd.Parameters.AddWithValue("@CourseId", enrollment.CourseId);
                inserted++;

                if (inserted % 10000 == 0)
                {
                    Console.WriteLine($"{inserted} students inserted...");
                }
                cmd.ExecuteNonQuery();
            }
        }

    }
}