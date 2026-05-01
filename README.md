University Data Management System
Overview

This is a C# console application using SQL Server and ADO.NET to manage university data such as students, courses, instructors, departments, and enrollments.

The main focus of the project is a high-performance student upsert pipeline for handling large datasets.

## Features
- Bulk data import using SqlBulkCopy
- Upsert (insert/update) using SQL MERGE
- Staging table pattern
- Transaction handling (commit/rollback)
- Batch processing for large datasets

## Project Structure
- DTOs/
- Models/
- Mapping/
- Services/
- Program.cs

## Configuration
Update before running:

csharp
string connectionString = "YOUR_CONNECTION_STRING";
string filePath = "PATH_TO_CSV_FILE";

## Notes
- CSV is used for better performance than Excel
- Large datasets are excluded using .gitignore
- Designed to handle 100K–1M+ records efficiently

## Tech Stack
- C# (.NET)
- SQL Server
- ADO.NET
- AutoMapper
