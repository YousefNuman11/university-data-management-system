using System.Data;
using Microsoft.Data.SqlClient;

namespace University_System.Services
{
    public class StudentsUpsertService
    {
        private readonly string _connectionString;

        public StudentsUpsertService(string connectionString)
        {
            _connectionString = connectionString;    
        }

        public void Import(string filePath)
        {
            var table = ReadCsv(filePath);

            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                TruncateStagingTable(connection, transaction);

                BulkInsertToStaginTable(table, connection, transaction);

                ExecuteSql(connection, transaction,
                    "CREATE INDEX IX_Students_Staging_StudentId ON Students_Staging(StudentId);");

                MergeStudents(connection, transaction);

                ExecuteSql(connection, transaction,
                    "DROP INDEX IX_Students_Staging_StudentId ON Students_Staging;");

                transaction.Commit();

                Console.WriteLine("Upsert completed successfully.");
            }
            catch(Exception ex)
            {
                transaction.Rollback();

                Console.WriteLine("Transaction rolled back.");
                Console.WriteLine(ex.Message);
            }
        }
        private DataTable ReadCsv(string filePath)
        {
            var table = new DataTable();

            table.Columns.Add("StudentId", typeof(int));
            table.Columns.Add("StudentName", typeof(string));
            table.Columns.Add("Major", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("Gender", typeof(string));
            table.Columns.Add("DateOfBirth", typeof(DateTime));
            table.Columns.Add("DepartmentId", typeof(int));

            using StreamReader reader = new StreamReader(filePath);

            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var values = line.Split(',');

                if (values.Length < 7)
                    continue;

                if (!int.TryParse(values[0], out int studentId))
                    continue;

                if (!int.TryParse(values[6], out int deptId))
                    continue;

                if (!DateTime.TryParse(values[5], out DateTime dob))
                    continue;

                table.Rows.Add(
                    studentId,
                    values[1],
                    values[2],
                    values[3],
                    values[4],
                    dob,
                    deptId
                );
            }
            return table;
        }

        private void TruncateStagingTable(SqlConnection connection, SqlTransaction transaction)
        {
            var sql = "TRUNCATE TABLE Students_Staging;";

            ExecuteSql(connection, transaction, sql);    
        }

        private void BulkInsertToStaginTable(DataTable table, SqlConnection connection, SqlTransaction transaction)
        {
            using SqlBulkCopy bulkCopy =
                new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);

            bulkCopy.DestinationTableName = "Students_Staging";
            bulkCopy.BatchSize = 50000;
            bulkCopy.BulkCopyTimeout = 300;

            bulkCopy.ColumnMappings.Add("StudentId", "StudentId");
            bulkCopy.ColumnMappings.Add("StudentName", "StudentName");
            bulkCopy.ColumnMappings.Add("Major", "Major");
            bulkCopy.ColumnMappings.Add("Email", "Email");
            bulkCopy.ColumnMappings.Add("Gender", "Gender");
            bulkCopy.ColumnMappings.Add("DateOfBirth", "DateOfBirth");
            bulkCopy.ColumnMappings.Add("DepartmentId", "DepartmentId");

            bulkCopy.WriteToServer(table);
        }


        private void MergeStudents(SqlConnection connection, SqlTransaction transaction)
        {
            var sql = @"
            MERGE Students AS target
            USING Students_Staging AS source
            ON target.StudentId = source.StudentId

            WHEN MATCHED THEN 
                UPDATE SET 
                    target.StudentName = source.StudentName,
                    target.Major = source.Major,
                    target.Email = source.Email,
                    target.Gender = source.Gender,
                    target.DateOfBirth = source.DateOfBirth,
                    target.DepartmentId = source.DepartmentId

            WHEN NOT MATCHED BY TARGET THEN
                    INSERT
                (
                    StudentId,
                    StudentName,
                    Major,
                    Email,
                    Gender,
                    DateOfBirth,
                    DepartmentId
                )
                VALUES
                (
                    source.StudentId,
                    source.StudentName,
                    source.Major,
                    source.Email,
                    source.Gender,
                    source.DateOfBirth,
                    source.DepartmentId
                );";

            ExecuteSql(connection, transaction, sql);
        }
        private void ExecuteSql(SqlConnection connection, SqlTransaction transaction, string sql)
        {
            using SqlCommand cmd = new SqlCommand(sql, connection, transaction);

            cmd.CommandTimeout = 300;
            cmd.ExecuteNonQuery();
        }
    }
}
