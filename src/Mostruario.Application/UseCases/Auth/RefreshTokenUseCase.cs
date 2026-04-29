using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Auth;

public class RefreshTokenUseCase(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUnitOfWork unitOfWork)
    : IRefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthResponse> ExecuteAsync(RefreshTokenRequest request)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedToken == null || storedToken.IsExpired || storedToken.IsRevoked)
        {
            throw new BusinessRuleException("Invalid refresh token");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
        {
            throw new BusinessRuleException("User is not active");
        }

        await _refreshTokenRepository.RevokeAsync(storedToken);

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var (refreshToken, refreshExpiresAt) = _refreshTokenGenerator.GenerateToken();

        await _refreshTokenRepository.AddAsync(
            new Domain.Entities.RefreshToken(user.Id, refreshToken, refreshExpiresAt));
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }
}
