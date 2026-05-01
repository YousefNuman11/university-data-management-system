using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class ExamService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;
        public ExamService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var examDtos = ReadExamsFromCsv(filePath);
            Console.WriteLine($"Read {examDtos.Count} rows from CSV.");

            var exams = _mapper.Map<List<Exam>>(examDtos);
            InsertExamsInDatabase(exams);
        }

        private List<ExamCsvDto> ReadExamsFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim()

            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<ExamCsvDto>().ToList();
        }
        private void InsertExamsInDatabase(List<Exam> exams)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var exam in exams)
            {
                string query = @"
                    INSERT INTO Exams(Type, Date, TotalMarks, CourseId)
                    VALUES(@Type, @Date, @TotalMarks, @CourseId)";

                using SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Type", exam.Type);
                cmd.Parameters.AddWithValue("@Date", exam.Date);
                cmd.Parameters.AddWithValue("@TotalMarks", exam.TotalMarks);
                cmd.Parameters.AddWithValue("@CourseId", exam.CourseId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}