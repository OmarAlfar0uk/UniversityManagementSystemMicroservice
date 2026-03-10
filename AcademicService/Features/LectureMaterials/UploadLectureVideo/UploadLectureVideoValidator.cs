using FluentValidation;

namespace AcademicService.Features.LectureMaterials.UploadLectureVideo;

public class UploadLectureVideoValidator : AbstractValidator<UploadLectureVideoCommand>
{
    public UploadLectureVideoValidator()
    {
        RuleFor(x => x.LectureId).NotEmpty();
    }
}
