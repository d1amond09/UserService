using FluentValidation;
using MediatR;
using ValidationException = UserService.Application.Common.Exceptions.ValidationException;

namespace UserService.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) :
	IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken ct)
	{
		if (!_validators.Any())
			return await next(ct);

		var context = new ValidationContext<TRequest>(request);
		
		var errorsDictionary = _validators
			.Select(x => x.Validate(context))
			.SelectMany(x => x.Errors)
			.Where(x => x != null)
			.GroupBy(
				x => x.PropertyName[(x.PropertyName.IndexOf('.') + 1)..],
				x => x.ErrorMessage,
				(propertyName, errorMessages) => new
				{
					Key = propertyName,
					Values = errorMessages.Distinct().ToArray()
				})
			.ToDictionary(x => x.Key, x => x.Values);
		
		if (errorsDictionary.Count > 0)
			throw new ValidationException(errorsDictionary);
		
		return await next(ct);
	}
}
