using MediatR;

namespace AuthService.Features.Auth.Student.GetProfile;

public record GetProfileQuery(Guid StudentId) : IRequest<StudentProfileResponse>;
