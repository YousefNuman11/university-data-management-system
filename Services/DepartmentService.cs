using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Globalization;
using University_System.DTOs;
using University_System.Models;

namespace University_System.Services
{
    public class DepartmentService
    {
        private readonly IMapper _mapper;
        private readonly string _connectionString;

        public DepartmentService(IMapper mapper, string connectionString)
        {
            _mapper = mapper;
            _connectionString = connectionString;
        }

        public void Import(string filePath)
        {
            var departmentDtos = ReadDepartmentsFromCsv(filePath);
            Console.WriteLine($"Read {departmentDtos.Count} rows from CSV.");

            var departments = _mapper.Map<List<Department>>(departmentDtos);
            InsertDepartmentsIntoDatabase(departments);
        }

        private List<DepartmentCsvDto> ReadDepartmentsFromCsv(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.Trim()
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<DepartmentCsvDto>().ToList();
        }

        private void InsertDepartmentsIntoDatabase(List<Department> departments)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var department in departments)
            {
                string query = @"
                    INSERT INTO Department (DepartmentName, DateOfEstablishment)
                    VALUES (@DepartmentName, @DateOfEstablishment)";

                using SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                cmd.Parameters.AddWithValue("@DateOfEstablishment", department.EstablishmentDate);

                cmd.ExecuteNonQuery();
            }
        }
    }
}