namespace ExamService.Contracts;

public record PagedResponse<T>(
    IEnumerable<T> Data,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages
);
