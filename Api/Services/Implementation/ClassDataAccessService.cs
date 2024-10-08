using Database;
using Domain.Model;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementation;

public class ClassDataAccessService : IClassDataAccessService
{
    private readonly ApiContext _context;
    private readonly IValidator<(Class cls, Student student)> _enrollmentValidator;    

    public ClassDataAccessService(ApiContext context, 
        IValidator<(Class cls, Student student)> enrollmentValidator)
    {
        _context = context;
        _enrollmentValidator = enrollmentValidator;        
    }

    public async Task<Class> CreateClassAsync(Class newClass)
    {
        _context.Add(newClass);
        await _context.SaveChangesAsync();

        return newClass;
    }

    public async Task<Result<Class>> GetClassByIdAsync(int id)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);

        if (cls == null)
        {
            var invalidOperationException = new InvalidOperationException($"Class doesn't exist with Id {id}");
            return new Result<Class>(invalidOperationException);
        }

        return cls;
    }

    public async Task<Result<Student>> EnrollStudentAsync(int classId, int studentId)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
        var student = await _context.Students.FirstOrDefaultAsync(c => c.Id == studentId);

        var validationResult = await _enrollmentValidator.ValidateAsync((cls,student));

        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(validationResult.Errors);
            return new Result<Student>(validationException);
        }

        cls.Students.Add(student);
        student.Classes.Add(cls);

        await _context.SaveChangesAsync();

        return student;
    }
}
