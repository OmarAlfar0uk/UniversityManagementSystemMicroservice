using FluentValidation;

namespace AcademicService.Features.LectureMaterials.GetLecturePdf;

public class GetLecturePdfValidator : AbstractValidator<GetLecturePdfQuery>
{
    public GetLecturePdfValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
