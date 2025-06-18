namespace UserService.Application.Common.Exceptions;

public sealed class SendEmaiMessageException() : 
	BadRequestException("Error with SmtpClient during sending email message")
{
}
