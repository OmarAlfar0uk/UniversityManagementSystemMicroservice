namespace AcademicService.Middlewares;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
