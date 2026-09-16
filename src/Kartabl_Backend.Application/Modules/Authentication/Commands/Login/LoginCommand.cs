using System.Text.Json.Serialization;
using Ardalis.Result;
using MediatR;

namespace Kartabl_Backend.Application.Modules.Authentication.Commands.Login;

public record LoginCommand(
    string NationalCode,
    string Password) : IRequest<Result<LoginResponse>>;

public record LoginUserDto(
    int Id,
    string NationalCode,
    string FirstName,
    string LastName,
    string Role,
    List<string> Permissions);

public record LoginResponse(
    string AccessToken,
    LoginUserDto User,
    [property: JsonIgnore] string RawRefreshToken, // Excluded from JSON payload automatically    string TokenType = "Bearer",
    string TokenType = "Bearer",
    int ExpiresInSeconds = 900);