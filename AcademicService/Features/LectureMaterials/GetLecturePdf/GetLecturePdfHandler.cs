using AcademicService.Contracts;
using MediatR;

namespace AcademicService.Features.LectureMaterials.GetLecturePdf;

public class GetLecturePdfHandler : IRequestHandler<GetLecturePdfQuery, PdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLecturePdfHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PdfResponse> Handle(GetLecturePdfQuery request, CancellationToken cancellationToken)
    {
        var pdf = await _unitOfWork.LecturePdfs.FindAsync(p => p.LectureId == request.LectureId);
        if (pdf is null)
            throw new KeyNotFoundException($"No PDF found for lecture {request.LectureId}.");

        return new PdfResponse(pdf.Id, pdf.FileUrl, pdf.LectureId);
    }
}
