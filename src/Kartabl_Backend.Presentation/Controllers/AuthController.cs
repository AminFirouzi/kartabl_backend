using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Kartabl_Backend.Application.Modules.Authentication.Commands.Login;
using Kartabl_Backend.Application.Modules.Authentication.Commands.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kartabl_Backend.Presentation.Controllers;

public class AuthController : ApiController
{
    /// <summary>
    /// Authenticates staff user and returns JWT access token along with an HTTP-Only Refresh Cookie.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command, 
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            Response.Cookies.Append("refreshToken", result.Value.RawRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/v1/auth",
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }

        return this.ToActionResult(result);
    }

    /// <summary>
    /// Generates a new JWT access token using the refreshToken HTTP-only cookie.
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(CancellationToken cancellationToken)
    {
        // 1. Extract refreshToken from HTTP-only cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var rawRefreshToken) || string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            return Unauthorized(new { message = "Refresh token cookie is missing." });
        }

        // 2. Dispatch command with the cookie value
        var command = new RefreshTokenCommand(rawRefreshToken);
        var result = await Mediator.Send(command, cancellationToken);

        // 3. If invalid or revoked, clear the cookie on client side
        if (result.Status == ResultStatus.Unauthorized)
        {
            Response.Cookies.Delete("refreshToken");
        }

        return this.ToActionResult(result);
    }
}