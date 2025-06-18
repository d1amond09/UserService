namespace UserService.Application.Common.Exceptions;

public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errors) :
    BadRequestException("One or more validation errors occurred")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
