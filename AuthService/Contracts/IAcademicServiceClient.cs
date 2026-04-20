namespace AuthService.Contracts
{
    public interface IAcademicServiceClient
    {
        Task<bool> DeleteStudentEnrollmentsAsync(
            Guid studentId,
            CancellationToken cancellationToken);
    }
}
