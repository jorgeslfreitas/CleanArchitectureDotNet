using Mostruario.Application.DTOs;
using Mostruario.Domain.Entities;

namespace Mostruario.Application.Interfaces.Auth;

public interface ILoginUseCase
{
    Task<AuthResponse> ExecuteAsync(LoginRequest request);
}

public interface IRegisterUseCase
{
    Task<AuthResponse> ExecuteAsync(RegisterRequest request);
}

public interface IRefreshTokenUseCase
{
    Task<AuthResponse> ExecuteAsync(RefreshTokenRequest request);
}

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}

public interface IRefreshTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken();
}
