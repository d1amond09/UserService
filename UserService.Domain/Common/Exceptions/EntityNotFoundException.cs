namespace UserService.Domain.Common.Exceptions;

public class EntityNotFoundException(string entityName, object id) : 
	BadRequestException($"Entity \"{entityName}\" ({id}) was not found.")
{
}
