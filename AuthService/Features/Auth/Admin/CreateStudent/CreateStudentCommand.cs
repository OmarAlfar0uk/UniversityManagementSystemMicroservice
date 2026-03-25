using Auth_Service.Features.Shared;
using MediatR;

namespace AuthService.Features.Auth.Admin.CreateStudent
{
     public record CreateStudentCommand(
          string Email,
          string FirstName,
          string LastName,
          string Gender,
          Guid? DepartmentId
      ) : IRequest<EndpointResponse<CreateStudentResponse>>;
 }
