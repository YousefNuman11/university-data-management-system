using AutoMapper;
using CsvHelper;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class CourseService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public CourseService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var courseDtos = ReadCoursesFromCsv(filePath);
            Console.WriteLine($"Read {courseDtos.Count} courses from CSV.");

            var courses = _mapper.Map<List<Course>>(courseDtos);
            InsertCoursesIntoDatabase(courses);
        }

        private List<CourseCsvDto> ReadCoursesFromCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<CourseCsvDto>().ToList();
        }

        private void InsertCoursesIntoDatabase(List<Course> courses)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var course in courses)
            {
                string query = @"
                    INSERT INTO Courses (CourseName, CreditHours, DepartmentId)
                    VALUES (@CourseName, @CreditHours, @DepartmentId)";

                using SqlCommand cmd = new SqlCommand(query, connection);
       
                cmd.Parameters.AddWithValue("@CourseName", course.CourseName);
                cmd.Parameters.AddWithValue("@CreditHours", course.CreditHours);
                cmd.Parameters.AddWithValue("@DepartmentId", course.DepartmentId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}