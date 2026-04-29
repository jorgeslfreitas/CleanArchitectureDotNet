using FluentAssertions;
using Moq;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Application.UseCases.Auth;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.Tests.UseCases;

public class RefreshTokenUseCaseTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock = new();
    private readonly Mock<IRefreshTokenGenerator> _refreshTokenGeneratorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Execute_InvalidToken_ShouldThrow()
    {
        var request = new RefreshTokenRequest { RefreshToken = "missing" };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        var useCase = new RefreshTokenUseCase(
            _refreshTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _unitOfWorkMock.Object);

        var act = async () => await useCase.ExecuteAsync(request);

        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Execute_ValidToken_ShouldReturnNewTokens()
    {
        var user = new User("user@example.com", "hash");
        var storedToken = new RefreshToken(user.Id, "refresh-token", DateTime.UtcNow.AddDays(1));

        var request = new RefreshTokenRequest { RefreshToken = storedToken.Token };

        _refreshTokenRepositoryMock
            .Setup(x => x.GetByTokenAsync(request.RefreshToken))
            .ReturnsAsync(storedToken);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _jwtTokenGeneratorMock
            .Setup(x => x.GenerateToken(user))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(60)));

        _refreshTokenGeneratorMock
            .Setup(x => x.GenerateToken())
            .Returns(("new-refresh-token", DateTime.UtcNow.AddDays(7)));

        var useCase = new RefreshTokenUseCase(
            _refreshTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _jwtTokenGeneratorMock.Object,
            _refreshTokenGeneratorMock.Object,
            _unitOfWorkMock.Object);

        var result = await useCase.ExecuteAsync(request);

        result.Token.Should().Be("access-token");
        result.RefreshToken.Should().Be("new-refresh-token");

        _refreshTokenRepositoryMock.Verify(x => x.RevokeAsync(storedToken), Times.Once);
        _refreshTokenRepositoryMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
