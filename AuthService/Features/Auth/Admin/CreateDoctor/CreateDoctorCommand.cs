using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.CreateDoctor
{
    public record CreateDoctorCommand(
       string Email,
       string FirstName,
       string LastName,
       string Gender,
       string Department
   ) : IRequest<EndpointResponse<CreateDoctorResponse>>;
}
