using MediatR;

namespace AuthService.Features.Auth.Doctor.GetProfile;

public record GetProfileQuery(Guid DoctorId) : IRequest<DoctorProfileResponse>;
