using FluentValidation;

namespace AcademicService.Features.LectureMaterials.UploadLecturePdf;

public class UploadLecturePdfValidator : AbstractValidator<UploadLecturePdfCommand>
{
    public UploadLecturePdfValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
