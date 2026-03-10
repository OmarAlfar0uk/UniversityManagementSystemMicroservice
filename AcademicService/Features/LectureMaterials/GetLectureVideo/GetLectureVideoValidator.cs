using FluentValidation;

namespace AcademicService.Features.LectureMaterials.GetLectureVideo;

public class GetLectureVideoValidator : AbstractValidator<GetLectureVideoQuery>
{
    public GetLectureVideoValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
