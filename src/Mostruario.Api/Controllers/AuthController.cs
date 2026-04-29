using Microsoft.AspNetCore.Mvc;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;

namespace Mostruario.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(
    ILoginUseCase loginUseCase,
    IRegisterUseCase registerUseCase,
    IRefreshTokenUseCase refreshTokenUseCase)
    : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await loginUseCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await registerUseCase.ExecuteAsync(request);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await refreshTokenUseCase.ExecuteAsync(request);
        return Ok(result);
    }
}