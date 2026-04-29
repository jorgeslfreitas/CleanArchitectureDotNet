using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Auth;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Auth;

public class LoginUseCase(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    : ILoginUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthResponse> ExecuteAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
            throw new BusinessRuleException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Invalid email or password");

        if (!user.IsActive)
            throw new BusinessRuleException("User account is inactive");

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