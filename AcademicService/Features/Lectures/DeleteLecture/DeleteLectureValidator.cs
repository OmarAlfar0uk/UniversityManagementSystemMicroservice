using FluentValidation;

namespace AcademicService.Features.Lectures.DeleteLecture;

public class DeleteLectureValidator : AbstractValidator<DeleteLectureCommand>
{
    public DeleteLectureValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
