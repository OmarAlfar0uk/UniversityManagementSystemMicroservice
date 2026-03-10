using FluentValidation;

namespace AcademicService.Features.Lectures.GetLecturesByCourse;

public class GetLecturesByCourseValidator : AbstractValidator<GetLecturesByCourseQuery>
{
    public GetLecturesByCourseValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
