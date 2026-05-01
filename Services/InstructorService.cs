using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class InstructorService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        public InstructorService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var departmentDtos = ReadDepartmentsFromCsv(filePath);
            Console.WriteLine($"Read {departmentDtos.Count} rows from CSV.");

            var instructors = _mapper.Map<List<Instructor>>(departmentDtos);
            InsertDepartmentsIntoDatabase(instructors);
        }

        private List<InstructorCsvDto> ReadDepartmentsFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim()
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<InstructorCsvDto>().ToList();
        }
        private void InsertDepartmentsIntoDatabase(List<Instructor> instructors)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach(var instructor in instructors)
            {
                string query = @"
                    INSERT INTO Instructors(InstructorName, Email, Rank, DepartmentId)
                    VALUES (@InstructorName, @Email, @Rank, @DepartmentId)";
                using SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@InstructorName", instructor.InstructorName);
                cmd.Parameters.AddWithValue("@Email", instructor.Email);
                cmd.Parameters.AddWithValue("@Rank", instructor.Rank);
                cmd.Parameters.AddWithValue("@DepartmentId", instructor.DepartmentId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}