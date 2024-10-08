using Database;
using Domain.Model;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementation;

public class TeacherDataAccessService : ITeacherDataAccessService
{
    private readonly ApiContext _context;
    private readonly IValidator<(Teacher, Class)> _classAssignmentValidator;

    public TeacherDataAccessService(ApiContext context,
        IValidator<(Teacher, Class)> classAssignmentValidator)
    {
        _context = context;
        _classAssignmentValidator = classAssignmentValidator;
    }

    public async Task<Teacher> CreateTeacherAsync(Teacher newTeacher)
    {
        _context.Add(newTeacher);
        await _context.SaveChangesAsync();

        return newTeacher;
    }

    public async Task<Result<Teacher>> GetTeacherByIdAsync(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(c => c.Id == id);

        if (teacher == null)
        {
            var invalidOperationException = new InvalidOperationException($"Teacher doesn't exist with Id {id}");
            return new Result<Teacher>(invalidOperationException);
        }

        return teacher;
    }

    public async Task<Result<Teacher>> AssignClassAsync(int teacherId, int classId)
    {
        var cls = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);
        var teacher = await _context.Teachers.Include(t => t.Classes).FirstOrDefaultAsync(c => c.Id == teacherId);

        // indempotent if teacher is already assigned this class
        if (teacher.Classes.Any(c => c.Id == classId))
            return teacher;

        var validationResult = await _classAssignmentValidator.ValidateAsync((teacher, cls));

        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(validationResult.Errors);
            return new Result<Teacher>(validationException);
        }

        cls.Teacher = teacher;
        teacher.Classes.Add(cls);

        await _context.SaveChangesAsync();

        return teacher;
    }
}
