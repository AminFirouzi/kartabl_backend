using Ardalis.Result;
using Kartabl_Backend.Application.Common.Interfaces;
using MediatR;

namespace Kartabl_Backend.Application.Modules.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Fetch user by National Code
        var user = await _unitOfWork.Users.GetByNationalCodeWithRoleAsync(request.NationalCode, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result.Unauthorized("Invalid credentials or inactive account.");
        }

        // 2. Extract permissions list
        var permissions = user.Role?.Permissions?
            .Select(p => $"Permissions.Documents.{p}") // Format permissions according to your domain requirements
            .ToList() ?? new List<string>();

        // 3. Issue Access Token & Refresh Token
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, permissions);
        var rawRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var hashedRefreshToken = _jwtTokenGenerator.HashRefreshToken(rawRefreshToken);

        // 4. Persist Refresh Token Entity
        var refreshTokenEntity = Domain.Entities.RefreshToken.Create(
            tokenHash: hashedRefreshToken,
            expiresAt: DateTime.UtcNow.AddDays(7),
            userId: user.Id);

        await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 5. Build DTOs matching contract
        var userDto = new LoginUserDto(
            Id: user.Id,
            NationalCode: user.NationalCode,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Role: user.RoleId.ToString(), // Converts UserRole enum (SuperUser, Expert, Employee) directly to string
            Permissions: permissions
        );
        int expiresInSeconds = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        return Result.Success(new LoginResponse(
            AccessToken: accessToken,
            User: userDto,
            RawRefreshToken: rawRefreshToken,
            TokenType: "Bearer",
            ExpiresInSeconds: expiresInSeconds > 0 ? expiresInSeconds : 900));
    }
}