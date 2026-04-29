using FluentAssertions;
using Moq;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Application.UseCases.Auth;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.Tests.UseCases;

public class RegisterUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<IRefreshTokenGenerator> _refreshTokenGeneratorMock = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Execute_ValidRequest_ShouldReturnTokens()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "123456"
        };

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(false);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(60)));

        _refreshTokenGeneratorMock
            .Setup(x => x.GenerateToken())
            .Returns(("refresh-token", DateTime.UtcNow.AddDays(7)));

        var useCase = new RegisterUseCase(
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _refreshTokenRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var result = await useCase.ExecuteAsync(request);

        result.Token.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.Email.Should().Be(request.Email);

        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Execute_EmailAlreadyExists_ShouldThrow()
    {
        var request = new RegisterRequest
        {
            Email = "user@example.com",
            Password = "123456"
        };

        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(request.Email))
            .ReturnsAsync(true);

        var useCase = new RegisterUseCase(
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _refreshTokenRepositoryMock.Object,
            _unitOfWorkMock.Object);

        var act = async () => await useCase.ExecuteAsync(request);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }
}
