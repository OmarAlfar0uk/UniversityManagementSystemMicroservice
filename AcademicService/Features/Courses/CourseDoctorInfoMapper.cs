using AcademicService.Contracts;
using AcademicService.Data.Models;
using AcademicService.Dtos;

namespace AcademicService.Features.Courses;

internal static class CourseDoctorInfoMapper
{
    public static async Task EnrichAsync(IEnumerable<Course> courses, IAuthServiceClient authServiceClient)
    {
        var doctorIds = courses
            .Select(c => c.DoctorId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var doctors = new Dictionary<Guid, UserInfoDto>();
        foreach (var doctorId in doctorIds)
        {
            var doctor = await authServiceClient.GetUserInfoAsync(doctorId);
            if (doctor is not null)
                doctors[doctorId] = doctor;
        }

        foreach (var course in courses)
        {
            if (!doctors.TryGetValue(course.DoctorId, out var doctor))
                continue;

            course.DoctorFirstName = doctor.FirstName ?? string.Empty;
            course.DoctorFullName = doctor.FullName ?? string.Empty;
            course.DoctorEmail = doctor.Email ?? string.Empty;
        }
    }

    public static async Task EnrichAsync(Course course, IAuthServiceClient authServiceClient)
    {
        await EnrichAsync([course], authServiceClient);
    }
}
