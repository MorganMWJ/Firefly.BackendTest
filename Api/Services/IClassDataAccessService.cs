using Domain.Model;
using LanguageExt.Common;

namespace Api.Services;

public interface IClassDataAccessService
{
    public Task<Class> CreateClassAsync(Class newClass);

    public Task<Result<Class>> GetClassByIdAsync(int id);

    public Task<Result<Student>> EnrollStudentAsync(int classId, int studentId);
}
