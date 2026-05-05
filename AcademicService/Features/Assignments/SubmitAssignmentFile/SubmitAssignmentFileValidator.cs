using FluentValidation;

namespace AcademicService.Features.Assignments.SubmitAssignmentFile;

public class SubmitAssignmentFileValidator : AbstractValidator<SubmitAssignmentFileCommand>
{
    public SubmitAssignmentFileValidator()
    {
        RuleFor(x => x.AssignmentOrLectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.File).NotNull();
    }
}
