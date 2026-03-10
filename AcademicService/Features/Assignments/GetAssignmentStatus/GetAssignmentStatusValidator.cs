using FluentValidation;

namespace AcademicService.Features.Assignments.GetAssignmentStatus;

public class GetAssignmentStatusValidator : AbstractValidator<GetAssignmentStatusQuery>
{
    public GetAssignmentStatusValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
    }
}
