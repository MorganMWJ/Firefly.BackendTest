using Domain.Model;
using FluentValidation;
using System.Net;

namespace Api.Validation;

public class StudentEnrollmentValidator : AbstractValidator<(Class cls, Student student)>
{
    public StudentEnrollmentValidator()
    {
        RuleFor(x => x.cls)
           .NotNull().WithMessage("Class doesn't exist.")
           .WithState(_ => HttpStatusCode.NotFound)
           .DependentRules(() =>
           {
               RuleFor(x => x.cls)
                .Must((model, cls) => model.cls.Students.Count < cls.Capacity)
                .WithMessage("A student cannot be enrolled in a class that is over its capacity.")
                .WithState(_ => HttpStatusCode.BadRequest);
           });

        RuleFor(x => x.student)
            .NotNull().WithMessage("Student doesn't exist.")
            .WithState(_ => HttpStatusCode.NotFound);
    }
}
