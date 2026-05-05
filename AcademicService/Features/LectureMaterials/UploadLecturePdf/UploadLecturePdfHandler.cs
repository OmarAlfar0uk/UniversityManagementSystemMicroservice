using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Features.LectureMaterials.GetLecturePdf;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AcademicService.Features.LectureMaterials.UploadLecturePdf;

public class UploadLecturePdfHandler : IRequestHandler<UploadLecturePdfCommand, PdfResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileHelper _fileHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadLecturePdfHandler(IUnitOfWork unitOfWork, IFileHelper fileHelper, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _fileHelper = fileHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PdfResponse> Handle(UploadLecturePdfCommand request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            throw new UnauthorizedAccessException("User not authenticated.");

        var doctorId = Guid.Parse(user.FindFirst("id")?.Value ?? throw new InvalidOperationException("Doctor ID claim not found."));

        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId);
        if (lecture is null)
            throw new KeyNotFoundException($"Lecture {request.LectureId} not found.");

        var course = await _unitOfWork.Courses.GetByIdAsync(lecture.CourseId);
        if (course is null)
            throw new KeyNotFoundException($"Course {lecture.CourseId} not found.");

        if (course.DoctorId != doctorId)
            throw new UnauthorizedAccessException("You do not own this lecture.");

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
