namespace GradeService.Contracts
{
    public interface IMailKitEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendActivationEmailAsync(string toEmail, string userName, string activationCode, string role, string universityId);
        Task SendOtpEmailAsync(string toEmail, string otpCode);
    }
}
