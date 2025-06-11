namespace UserService.Application.Common.Responses;

public class ApiNotFoundResponse(string name) : ApiBaseResponse(false)
{
    public string Message { get; set; } = $"ERROR: {name} is not found";
}
