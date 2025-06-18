using System.Threading;
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

		var validationResults = await Task.WhenAll(
		   _validators.Select(v => v.ValidateAsync(context, ct)));

		var failures = validationResults
			.SelectMany(r => r.Errors)
			.Where(f => f != null)
			.ToList();

		if (failures.Count != 0)
		{
			var errorsDictionary = failures
				.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
				.ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

			throw new ValidationException(errorsDictionary);
		}
		
		return await next(ct);
	}
}
