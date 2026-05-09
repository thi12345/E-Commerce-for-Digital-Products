using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ShopApp.Application.Auth.Commands.ChangePassword;
using ShopApp.Application.Auth.Commands.Login;
using ShopApp.Application.Auth.Commands.Register;
using ShopApp.Application.Auth.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Tests.Application;

public sealed class AuthCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    public AuthCommandHandlerTests()
    {
        _jwtTokenGenerator.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(new AuthTokenDto("access-token", "Bearer", DateTime.UtcNow.AddHours(1)));
    }

    [Fact]
    public async Task Login_ShouldReturnAuthResult_WhenCredentialsAreValid()
    {
        var user = User.Create("customer@example.com", "Customer", "hashed-password");
        _userRepository.Setup(r => r.GetByEmailAsync("customer@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("password", "hashed-password"))
            .Returns(true);

        var handler = new LoginCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _jwtTokenGenerator.Object,
            NullLogger<LoginCommandHandler>.Instance);

        var result = await handler.Handle(new LoginCommand("customer@example.com", "password"), CancellationToken.None);

        result.User.Email.Should().Be("customer@example.com");
        result.Token.AccessToken.Should().Be("access-token");
    }

    [Fact]
    public async Task Login_ShouldThrowDomainException_WhenPasswordIsInvalid()
    {
        var user = User.Create("customer@example.com", "Customer", "hashed-password");
        _userRepository.Setup(r => r.GetByEmailAsync("customer@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("wrong-password", "hashed-password"))
            .Returns(false);

        var handler = new LoginCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _jwtTokenGenerator.Object,
            NullLogger<LoginCommandHandler>.Instance);

        var act = () => handler.Handle(new LoginCommand("customer@example.com", "wrong-password"), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task Register_ShouldCreateUserAndReturnToken_WhenEmailIsAvailable()
    {
        _userRepository.Setup(r => r.GetByEmailAsync("new@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _passwordHasher.Setup(h => h.Hash("password"))
            .Returns("hashed-password");
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new RegisterCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _jwtTokenGenerator.Object,
            _unitOfWork.Object,
            NullLogger<RegisterCommandHandler>.Instance);

        var result = await handler.Handle(new RegisterCommand("new@example.com", "New User", "password"), CancellationToken.None);

        result.User.Email.Should().Be("new@example.com");
        result.Token.TokenType.Should().Be("Bearer");
        _userRepository.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new@example.com"), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_ShouldUpdatePasswordAndRotateSecurityStamp()
    {
        var user = User.Create("customer@example.com", "Customer", "old-hash");
        var originalSecurityStamp = user.SecurityStamp;

        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("old-password", "old-hash"))
            .Returns(true);
        _passwordHasher.Setup(h => h.Hash("new-password"))
            .Returns("new-hash");
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new ChangePasswordCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _unitOfWork.Object,
            NullLogger<ChangePasswordCommandHandler>.Instance);

        await handler.Handle(new ChangePasswordCommand(user.Id, "old-password", "new-password"), CancellationToken.None);

        user.PasswordHash.Should().Be("new-hash");
        user.SecurityStamp.Should().NotBe(originalSecurityStamp);
        _userRepository.Verify(r => r.Update(user), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_ShouldThrowDomainException_WhenCurrentPasswordIsInvalid()
    {
        var user = User.Create("customer@example.com", "Customer", "old-hash");
        _userRepository.Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("wrong-password", "old-hash"))
            .Returns(false);

        var handler = new ChangePasswordCommandHandler(
            _userRepository.Object,
            _passwordHasher.Object,
            _unitOfWork.Object,
            NullLogger<ChangePasswordCommandHandler>.Instance);

        var act = () => handler.Handle(new ChangePasswordCommand(user.Id, "wrong-password", "new-password"), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Current password is invalid.");
        _userRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
