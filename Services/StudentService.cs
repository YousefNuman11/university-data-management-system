using System.Globalization;
using AutoMapper;
using CsvHelper;
using Microsoft.Data.SqlClient;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services;

public class StudentService
{
    private readonly IMapper _mapper;
    private readonly string _connectionString;

    public StudentService(IMapper mapper, string connectionString)
    {
        _mapper = mapper;
        _connectionString = connectionString;
    }

    public void Import(string filePath)
    {
        Console.WriteLine("Reading CSV...");

        var studentDtos = ReadStudentCsv(filePath);
        Console.WriteLine($"Read {studentDtos.Count} students.");

        var students = _mapper.Map<List<Student>>(studentDtos);
        InsertIntoDatabase(students);
    }

    private List<StudentCsvDto> ReadStudentCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<StudentCsvDto>().ToList();
    }

    private void InsertIntoDatabase(List<Student> students)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        int inserted = 0;

        foreach (var student in students)
        {
            try
            {
                string query = @"
                    INSERT INTO Students
                    (StudentName, Major, Email, Gender, DateOfBirth, DepartmentId)
                    VALUES
                    (@StudentName, @Major, @Email, @Gender, @DateOfBirth, @DepartmentId)";

                using SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@StudentName", student.StudentName);
                cmd.Parameters.AddWithValue("@Major", student.Major);
                cmd.Parameters.AddWithValue("@Email", student.Email);
                cmd.Parameters.AddWithValue("@Gender", student.Gender);
                cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
                cmd.Parameters.AddWithValue("@DepartmentId", student.DepartmentId);

                cmd.ExecuteNonQuery();
                inserted++;

                if (inserted % 10000 == 0)
                {
                    Console.WriteLine($"{inserted} students inserted...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed StudentId={student.StudentId}");
                Console.WriteLine(ex.Message);
            }
        }
        Console.WriteLine($"Finished. Total inserted: {inserted}");
    }
}