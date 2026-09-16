using Ardalis.Result;
using MediatR;

namespace Kartabl_Backend.Application.Modules.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;

public record RefreshTokenResponse(
    string AccessToken,
    string TokenType = "Bearer",
    int ExpiresInSeconds = 900);