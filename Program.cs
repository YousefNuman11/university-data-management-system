using AutoMapper;
using Microsoft.Data.SqlClient;
using University_System.Mapping;
using University_System.Services;

class Program
{
    static void Main(string[] args)
    {
        string connectionString =
            "Server=localhost;Database=University_System;Trusted_Connection=True;TrustServerCertificate=True;";
        string filePath = @"C:\Users\Yousef\source\repos\T2-Training\University_System\UniversityData\students_1M_Upsert.csv";

        var service = new StudentsUpsertService(connectionString);

        service.Import(filePath);

        //InsertData(connectionString, filePath);
        //ReadData(connectionString);
    }

    static void InsertData(string connectionString, string filePath)
    {
        var Config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CourseInstructorProfile>();
        });

        Config.AssertConfigurationIsValid();
        IMapper mapper = Config.CreateMapper();

        var CourseInstructorService = new CourseInstructorService(mapper, connectionString);

        try
        {
            CourseInstructorService.Import(filePath);
            Console.WriteLine("All rows inserted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);
        }
    }

    static void ReadData(string connectionString)
    {
        string query = @"
        SELECT StudentId
        from Enrollment
        where CourseId in (102, 340)
        group by StudentId
        Having count(distinct CourseId) = 2;" ;

        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        using SqlCommand cmd = new SqlCommand(query, connection);
        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {


            Console.WriteLine($"{reader.GetString(0)} - {reader.GetInt32(1)} students");
        }

    }
}