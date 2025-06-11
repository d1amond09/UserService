using UserService.Application.Common.Responses;

namespace UserService.API.Extensions;

public static class ApiBaseResponseExtensions
{
	public static TResultType GetResult<TResultType>(this ApiBaseResponse apiBaseResponse) =>
		((ApiOkResponse<TResultType>)apiBaseResponse).Result;

}
