using Database;
using Domain.Model;
using FluentAssertions.Equivalency.Tracing;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Api.Tests.Component;

public class ControllerTestsBase : IDisposable
{
    protected readonly TestApplicationFactory _factory;
    protected readonly ApiContext _dbContext;
    protected readonly HttpClient _client;
    protected readonly Faker _faker;

    private readonly TeacherFaker _teacherFaker = new TeacherFaker();
    private readonly StudentFaker _studentFaker = new StudentFaker();
    private readonly ClassFaker _classFaker = new ClassFaker();

    public ControllerTestsBase()
    {
        _factory = new TestApplicationFactory();
        _client = _factory.CreateClient();
        _faker = new Faker();
        _dbContext = _factory.InMemoryContext;

        // Ensure database is created and seeded with data
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();
        SeedDatabase(_dbContext);
    }

    private void SeedDatabase(ApiContext dbContext)
    {
        // Check if data already exists to avoid duplication
        if (!dbContext.Classes.Any())
        {
            var seedClasses = _classFaker.Generate(3);
            dbContext.Classes.AddRange(seedClasses);
            dbContext.SaveChanges();
        }
        
        if (!dbContext.Teachers.Any())
        {
            var seedteachers = _teacherFaker.Generate(3);
            dbContext.Teachers.AddRange(seedteachers);
            dbContext.SaveChanges();
        }

        if (!dbContext.Students.Any())
        {
            var seedStudents = _studentFaker.Generate(6);
            dbContext.Students.AddRange(seedStudents);
            dbContext.SaveChanges();
        }
    }

    protected void SeedTeacherWithMaxClasses()
    {
        var seedClasses = _classFaker
            .RuleFor(t => t.Name, f => $"CompSci {f.Random.AlphaNumeric(5)}")
            .Generate(5);
        _dbContext.Classes.AddRange(seedClasses);
        _dbContext.SaveChanges();

        var seedTeacher = _teacherFaker.Generate();
        _dbContext.Teachers.AddRange(seedTeacher);
        _dbContext.SaveChanges();

        var classesToAssign = _dbContext.Classes.Where(c => c.Name.StartsWith("CompSci"));
        var teacherToAssign = _dbContext.Teachers.Single(t => t.Id == seedTeacher.Id);

        foreach (Class cls in classesToAssign)
        {
            cls.Teacher = teacherToAssign;
            teacherToAssign.Classes.Add(cls);
            _dbContext.SaveChanges();
        }
    }

    protected void SeedOverCapacityClass(int classId)
    {        
        var overCapacityClass = _dbContext.Classes.Single(c => c.Id == classId);

        // just set capacity to 0 for now
        overCapacityClass.Capacity = 0;

        _dbContext.SaveChanges();
    }

    // Reset the database by clearing data after test
    public void Dispose()
    {
        _dbContext.Classes.RemoveRange(_dbContext.Classes);
        _dbContext.Teachers.RemoveRange(_dbContext.Teachers);
        _dbContext.Students.RemoveRange(_dbContext.Students);
        _dbContext.SaveChanges();
        _dbContext.Database.EnsureDeleted();
    }
}
