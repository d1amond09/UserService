using System.ComponentModel.DataAnnotations;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using UserService.Application.Common.Enums;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Helpers;
using UserService.Application.Common.Notifications;
using UserService.Application.UseCases.Users.UpdateUserStatus;
using UserService.Domain.Users;
using Xunit;
using ValidationException = UserService.Application.Common.Exceptions.ValidationException;

namespace UserService.Tests.Application.UseCases.Users.Commands;

public class UpdateUserStatusCommandHandlerTests
{
	private readonly Mock<UserManager<User>> _userManagerMock;
	private readonly Mock<IPublisher> _publisherMock;
	private readonly UpdateUserStatusCommandHandler _handler;
	private readonly Fixture _fixture;

	public UpdateUserStatusCommandHandlerTests()
	{
		_fixture = new Fixture();

		var store = new Mock<IUserStore<User>>();
		_userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

		_publisherMock = new Mock<IPublisher>();

		_handler = new UpdateUserStatusCommandHandler(
			_userManagerMock.Object,
			_publisherMock.Object);
	}

	[Fact]
	public async Task Handle_ShouldBlockUserAndPublishNotification_WhenActionIsBlock()
	{
		// Arrange
		var userId = _fixture.Create<Guid>();
		var user = new User("test", "test@test.com") { Id = userId };
		var command = new UpdateUserStatusCommand(userId, UserStatusAction.Block);

		_userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
		_userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().NotThrowAsync();
		user.IsBlocked.Should().BeTrue();
		_userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);

		var expectedCacheKey = CacheKeyGenerator.GetUserCacheKey(userId);
		_publisherMock.Verify(p => p.Publish(
			It.Is<CacheInvalidationNotification>(n => n.CacheKey == expectedCacheKey),
			It.IsAny<CancellationToken>()),
			Times.Once);
	}


	[Fact]
	public async Task Handle_ShouldUnblockUser_WhenActionIsUnblock()
	{
		// Arrange
		var userId = _fixture.Create<Guid>();
		var user = new User("test", "test@test.com") { Id = userId };
		user.Block();

		var command = new UpdateUserStatusCommand(userId, UserStatusAction.Unblock);

		_userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
		_userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		user.IsBlocked.Should().BeFalse();
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenUserDoesNotExist()
	{
		// Arrange
		var command = _fixture.Create<UpdateUserStatusCommand>();

		_userManagerMock.Setup(um => um.FindByIdAsync(command.UserId.ToString())).ReturnsAsync((User?)null);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"User with ID '{command.UserId}' not found.");

		_publisherMock.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task Handle_ShouldThrowValidationException_WhenUpdateFails()
	{
		// Arrange
		var userId = _fixture.Create<Guid>();
		var user = new User("test", "test@test.com") { Id = userId };
		var command = new UpdateUserStatusCommand(user.Id, UserStatusAction.Block);

		var identityError = _fixture.Create<IdentityError>();
		var failedResult = IdentityResult.Failed(identityError);

		_userManagerMock.Setup(um => um.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
		_userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(failedResult);

		// Act
		Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
	}
}