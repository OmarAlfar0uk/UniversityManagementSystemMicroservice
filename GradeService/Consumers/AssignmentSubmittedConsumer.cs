using GradeService.Contracts;
using GradeService.Data.Models;
using MassTransit;
using Shered.Events;

namespace GradeService.Consumers
{
    public class AssignmentSubmittedConsumer : IConsumer<IAssignmentSubmitted>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGradeAuditLogger _auditLogger;

        public AssignmentSubmittedConsumer(IUnitOfWork unitOfWork, IGradeAuditLogger auditLogger)
        {
            _unitOfWork = unitOfWork;
            _auditLogger = auditLogger;
        }

        public async Task Consume(ConsumeContext<IAssignmentSubmitted> context)
        {
            var eventData = context.Message;

            // Logic: Create an initial grade record for the student if it doesn't exist
            // Or just log it for now as a demonstration of communication
            
            await _auditLogger.LogAsync(
                action: "ConsumeAssignmentSubmission",
                targetId: eventData.StudentId.ToString(),
                description: $"Received assignment submission event for Student: {eventData.StudentId}, Assignment: {eventData.AssignmentId}"
            );

            // Here you would typically add a record to the Grades table
            // Example:
            // var grade = new Grade { ... };
            // await _unitOfWork.Grades.AddAsync(grade);
            // await _unitOfWork.SaveChangesAsync();
        }
    }
}
