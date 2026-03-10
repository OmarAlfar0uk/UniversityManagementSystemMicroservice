using GradeService.Contracts;
using GradeService.Data.Models;
using MassTransit;
using Shered.Events;

namespace GradeService.Consumers
{
    public class QuizCompletedConsumer : IConsumer<IQuizCompleted>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGradeAuditLogger _auditLogger;

        public QuizCompletedConsumer(IUnitOfWork unitOfWork, IGradeAuditLogger auditLogger)
        {
            _unitOfWork = unitOfWork;
            _auditLogger = auditLogger;
        }

        public async Task Consume(ConsumeContext<IQuizCompleted> context)
        {
            var eventData = context.Message;
            
            await _auditLogger.LogAsync(
                action: "ConsumeQuizCompletion",
                targetId: eventData.StudentId.ToString(),
                description: $"Received quiz completion event for Student: {eventData.StudentId}, Quiz: {eventData.QuizId}, Score: {eventData.Score}%"
            );

            // Logic to update student's grade for this quiz in GradeService
        }
    }
}
