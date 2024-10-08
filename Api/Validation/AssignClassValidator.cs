using Domain.Model;
using FluentValidation;
using System.Net;

namespace Api.Validation
{
    public class AssignClassValidator : AbstractValidator<(Teacher teacher, Class cls)>
    {
        public AssignClassValidator() 
        {
            RuleFor(x => x.cls)
               .NotNull().WithMessage("Class doesn't exist.")
               .WithState(_ => HttpStatusCode.NotFound);

            RuleFor(x => x.teacher)
               .NotNull().WithMessage("Teacher doesn't exist.")
               .WithState(_ => HttpStatusCode.NotFound)
               .DependentRules(() =>
               {
                   RuleFor(x => x.teacher)
                    .Must((model, cls) => model.teacher.Classes.Count() < 5)
                    .WithMessage("A teacher cannot be assigned to more that 5 classes.")
                    .WithState(_ => HttpStatusCode.BadRequest);
               });
        }
    }
}
