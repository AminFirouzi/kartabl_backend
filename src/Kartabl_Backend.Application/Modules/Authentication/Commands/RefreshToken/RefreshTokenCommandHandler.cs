using Ardalis.Result;
using Kartabl_Backend.Application.Common.Interfaces;
using MediatR;

namespace Kartabl_Backend.Application.Modules.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Hash incoming token to locate it in DB
        var hashedToken = _jwtTokenGenerator.HashRefreshToken(request.RefreshToken);
        var existingToken = await _unitOfWork.RefreshTokens.GetByHashAsync(hashedToken, cancellationToken);

        // 2. Validate token existence and active state (401 Unauthorized)
        if (existingToken is null || !existingToken.IsActive)
        {
            return Result.Unauthorized("Token missing, invalid, or revoked.");
        }

        var user = existingToken.User;
        if (user is null)
        {
            return Result.Unauthorized("User associated with token not found.");
        }

        // 3. Validate user account status (403 Forbidden)
        if (!user.IsActive || user.IsDeleted)
        {
            return Result.Forbidden("Account disabled or inactive.");
        }

        // 4. Issue fresh Access Token
        var permissions = user.Role?.Permissions?
            .Select(p => p.ToString())
            .ToList() ?? new List<string>();

        var (newAccessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, permissions);
        
        // Calculate remaining seconds (e.g., 900 seconds = 15 minutes)
        int expiresInSeconds = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        return Result.Success(new RefreshTokenResponse(
            AccessToken: newAccessToken,
            TokenType: "Bearer",
            ExpiresInSeconds: expiresInSeconds > 0 ? expiresInSeconds : 900));
    }
}