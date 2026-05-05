using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLecturePdf;

public class GetLecturePdfHandler : IRequestHandler<GetLecturePdfQuery, PdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;

    public GetLecturePdfHandler(IUnitOfWork unitOfWork, IFileHelper fileHelper)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
    }

    public async Task<PdfResponse> Handle(GetLecturePdfQuery request, CancellationToken cancellationToken)
    {
        var pdf = await _unitOfWork.LecturePdfs.FindAsync(p => p.LectureId == request.LectureId);
        if (pdf is null)
            throw new KeyNotFoundException($"No PDF found for lecture {request.LectureId}.");

        return new PdfResponse(
            pdf.Id,
            string.IsNullOrEmpty(pdf.FileUrl) ? string.Empty : _fileHelper.GetFileUrl(pdf.FileUrl),
            pdf.LectureId
        );
    }
}
