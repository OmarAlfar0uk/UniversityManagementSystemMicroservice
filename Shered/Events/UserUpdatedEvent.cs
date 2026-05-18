namespace Shered.Events;

public record UserUpdatedEvent(
    Guid UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Email
);
