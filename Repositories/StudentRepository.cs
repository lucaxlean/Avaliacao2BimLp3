using Avaliacao3BimLp3.Database;
using Avaliacao3BimLp3.Models;
using Microsoft.Data.Sqlite;
using Dapper;

namespace Avaliacao3BimLp3.Repositories;

class StudentRepository
{
    private readonly DatabaseConfig _databaseConfig;
    public StudentRepository(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }
    
    public Student Save(Student student)
    {
        using (var connection = new SqliteConnection(_databaseConfig.ConnectionString))
        {
            connection.Open();

            connection.Execute("INSERT INTO Students VALUES(@Registration, @Name, @City, @Former)", student);

            return student;
        }
    }

    public void Delete(string registration)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("DELETE FROM Students WHERE registration = @Registration", new { Registration = registration });
    }

    public void MarkAsFormed(string registration)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        connection.Execute("UPDATE Students SET former = true Where registration = @Registration", new { Registration = registration });
    }

    public List<Student> GetAll()
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<Student>("SELECT * FROM Students").ToList();

        return students;
    }

    public List<Student> GetAllFormed()
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<Student>("SELECT * FROM Students WHERE former = true").ToList();

        return students;
    }

    public List<Student> GetAllStudentByCity(string city)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<Student>("SELECT * FROM Students WHERE city LIKE @City", new{City = city + "%"}).ToList();

        return students;
    }

    public List<Student> GetAllByCities(string[] cities)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<Student>("SELECT * FROM Students WHERE city IN @Cities", new { Cities = cities }).ToList();

        return students;
    }

    public List<CountStudentGroupByAttribute> CountByCities()
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<CountStudentGroupByAttribute>("SELECT city AS attributeName, COUNT(registration) AS studentNumber FROM Students GROUP BY city;").ToList();

        return students;
    }

    public List<CountStudentGroupByAttribute> CountByFormed()
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var students = connection.Query<CountStudentGroupByAttribute>("SELECT former AS attributeName, COUNT(registration) AS studentNumber FROM Students GROUP BY former;").ToList();

        return students;
    }

    public bool ExistsByRegistration(string registration)
    {
        using var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();

        var existsById = connection.ExecuteScalar<bool>("SELECT COUNT(registration) FROM Students WHERE registration = @Registration", new { @Registration = registration });

        return existsById;
    }
}