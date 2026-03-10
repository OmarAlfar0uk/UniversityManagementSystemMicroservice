using FluentValidation;

namespace AcademicService.Features.Assignments.GetAssignment;

public class GetAssignmentValidator : AbstractValidator<GetAssignmentQuery>
{
    public GetAssignmentValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
