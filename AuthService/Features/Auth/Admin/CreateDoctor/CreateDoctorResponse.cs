namespace AuthService.Features.Auth.Admin.CreateDoctor
{
    public class CreateDoctorResponse
    {
        public Guid DoctorId { get; set; }
        public string Email { get; set; } = default!;
        public string ActivationCode { get; set; } = default!;
    }
}
