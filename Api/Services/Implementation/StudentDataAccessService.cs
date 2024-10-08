using Database;
using Domain.Model;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementation;

public class StudentDataAccessService : IStudentDataAccessService
{
    private readonly ApiContext _context;

    public StudentDataAccessService(ApiContext context)
    {
        _context = context;
    }

    public async Task<Student> CreateStudentAsync(Student newStudent)
    {
        _context.Add(newStudent);
        await _context.SaveChangesAsync();

        return newStudent;
    }

    public async Task<Result<Student>> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(c => c.Id == id);

        if (student == null)
        {
            var invalidOperationException = new InvalidOperationException($"Student doesn't exist with Id {id}");
            return new Result<Student>(invalidOperationException);
        }

        return student;
    }
}
