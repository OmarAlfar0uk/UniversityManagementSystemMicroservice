using FluentValidation;

namespace AcademicService.Features.Assignments.SubmitAssignmentUrl;

public class SubmitAssignmentUrlValidator : AbstractValidator<SubmitAssignmentUrlCommand>
{
    public SubmitAssignmentUrlValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.ProjectUrl).NotEmpty().MaximumLength(500);
    }
}
