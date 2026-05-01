using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class CourseInstructorService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public CourseInstructorService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var courseInstructorDtos = ReadCourseInstructorFromCsv(filePath);

            Console.WriteLine($"Read {courseInstructorDtos.Count} rows from CSV.");

            var courseInstructors = _mapper.Map<List<CourseInstructor>>(courseInstructorDtos);

            InsertCourseInstructorInDatabase(courseInstructors);
        }

        private List<CourseInstructorCsvDto> ReadCourseInstructorFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim()
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<CourseInstructorCsvDto>().ToList();
        }

        private void InsertCourseInstructorInDatabase(List<CourseInstructor> courseInstructors)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            var inserted = 0;

            foreach (var courseInstructor in courseInstructors)
            {
                string query = @"
                    INSERT INTO CourseInstructor (CourseId, InstructorId)
                    VALUES (@CourseId, @InstructorId)";

                using SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@CourseId", courseInstructor.CourseId);
                cmd.Parameters.AddWithValue("@InstructorId", courseInstructor.InstructorId);

                cmd.ExecuteNonQuery();

                inserted++;

                if (inserted % 10000 == 0)
                {
                    Console.WriteLine($"{inserted} course-instructor rows inserted...");
                }
            }
        }
    }
}