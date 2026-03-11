using AcademicService.Data;
using AcademicService.Data.Models;
using AcademicService.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace AcademicService.Seeding;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AcademicDbContext>();

        await context.Database.EnsureCreatedAsync(); // Ensure DB is created (in case migrations are not run)

        if (!await context.Courses.AnyAsync())
        {
            var doctorId = Guid.NewGuid(); // Fake doctor ID
            var studentId1 = Guid.NewGuid(); // Fake student IDs
            var studentId2 = Guid.NewGuid();

            // 1. Courses
            var course1 = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Introduction to Computer Science",
                Description = "A foundational course in CS.",
                CoverImageUrl = "https://example.com/cs101.jpg",
                DoctorId = doctorId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var course2 = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Advanced Mathematics",
                Description = "Calculus, Linear Algebra, and more.",
                CoverImageUrl = "https://example.com/math201.jpg",
                DoctorId = doctorId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Courses.AddRangeAsync(course1, course2);

            // 2. Course Enrollments
            var enrollment1 = new CourseEnrollment
            {
                Id = Guid.NewGuid(),
                CourseId = course1.Id,
                StudentId = studentId1,
                EnrolledAt = DateTime.UtcNow
            };

            var enrollment2 = new CourseEnrollment
            {
                Id = Guid.NewGuid(),
                CourseId = course1.Id,
                StudentId = studentId2,
                EnrolledAt = DateTime.UtcNow
            };

            var enrollment3 = new CourseEnrollment
            {
                Id = Guid.NewGuid(),
                CourseId = course2.Id,
                StudentId = studentId1,
                EnrolledAt = DateTime.UtcNow
            };

            await context.CourseEnrollments.AddRangeAsync(enrollment1, enrollment2, enrollment3);

            // 3. Lectures
            var lecture1 = new Lecture
            {
                Id = Guid.NewGuid(),
                Title = "CS101 - Lecture 1: Basics",
                OrderIndex = 1,
                ThumbnailUrl = "https://example.com/thumb1.jpg",
                CourseId = course1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var lecture2 = new Lecture
            {
                Id = Guid.NewGuid(),
                Title = "CS101 - Lecture 2: Data Types",
                OrderIndex = 2,
                ThumbnailUrl = "https://example.com/thumb2.jpg",
                CourseId = course1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Lectures.AddRangeAsync(lecture1, lecture2);

            // 4. Lecture Materials
            var pdf1 = new LecturePdf
            {
                Id = Guid.NewGuid(),
                FileUrl = "https://example.com/lecture1.pdf",
                LectureId = lecture1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var video1 = new LectureVideo
            {
                Id = Guid.NewGuid(),
                VideoUrl = "https://example.com/lecture1.mp4",
                DurationInMinutes = 45,
                LectureId = lecture1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.LecturePdfs.AddAsync(pdf1);
            await context.LectureVideos.AddAsync(video1);

            // 5. Assignments
            var assignment1 = new Assignment
            {
                Id = Guid.NewGuid(),
                Title = "Assignment 1: Variables",
                FileUrl = "https://example.com/assignment1.pdf",
                LectureId = lecture1.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Assignments.AddAsync(assignment1);

            // 6. Assignment Submissions
            var submission1 = new AssignmentSubmission
            {
                Id = Guid.NewGuid(),
                StudentId = studentId1,
                AssignmentId = assignment1.Id,
                FileUrl = "https://example.com/sub1.pdf",
                SubmittedAt = DateTime.UtcNow
            };

            await context.AssignmentSubmissions.AddAsync(submission1);

            // 7. Schedule
            var schedule1 = new Schedule
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://example.com/schedule1.jpg",
                Type = ScheduleType.ClassSchedule,
                AcademicYear = "2026-2027",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Schedules.AddAsync(schedule1);

            await context.SaveChangesAsync();
        }
    }
}
