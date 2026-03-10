using FluentValidation;

namespace AcademicService.Features.Assignments.SubmitAssignmentFile;

public class SubmitAssignmentFileValidator : AbstractValidator<SubmitAssignmentFileCommand>
{
    public SubmitAssignmentFileValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
