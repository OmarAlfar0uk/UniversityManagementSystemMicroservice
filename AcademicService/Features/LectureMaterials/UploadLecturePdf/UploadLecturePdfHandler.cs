using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.LectureMaterials.GetLecturePdf;
using MediatR;

namespace AcademicService.Features.LectureMaterials.UploadLecturePdf;

public class UploadLecturePdfHandler : IRequestHandler<UploadLecturePdfCommand, PdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;

    public UploadLecturePdfHandler(IUnitOfWork unitOfWork, IFileHelper fileHelper)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
    }

    public async Task<PdfResponse> Handle(UploadLecturePdfCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        // Remove existing PDF file and record if any
        var existing = await _unitOfWork.LecturePdfs.FindAsync(p => p.LectureId == request.LectureId);
        if (existing is not null)
        {
            _fileHelper.DeleteFile(existing.FileUrl);
            _unitOfWork.LecturePdfs.Remove(existing);
        }

        // Save new file
        var relativePath = await _fileHelper.SaveFileAsync(request.File, "Lectures");

        var pdf = new LecturePdf
        {
            Id = Guid.NewGuid(),
            LectureId = request.LectureId,
            FileUrl = relativePath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.LecturePdfs.AddAsync(pdf);
        await _unitOfWork.SaveChangesAsync();

        return new PdfResponse(pdf.Id, _fileHelper.GetFileUrl(pdf.FileUrl), pdf.LectureId);
    }
}
