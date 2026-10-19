using AlfaHackaton.Auth.Common;
using AlfaHackaton.Auth.Tokens;
using AlfaHackaton.Controllers.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AlfaHackaton.Controllers.Controllers;

[ApiController]
[Authorize]
public abstract class BaseAuthController : BaseController
{
    private const string XUserId = "X-User-Id";
    private const string XUserRole = "X-User-Role";
    private IHeaderDictionary Headers => HttpContext.Request.Headers;
    private TokenPayload? _tokenPayload;

    private Guid? _userId;
    private UserRoles? _userRole;

    protected UserRoles UserRole
    {
        get
        {
            if (_userRole is null)
            {
                _userRole = ResolveUserRole();
            }

            return _userRole.Value;
        }
    }

    protected Guid UserId
    {
        get
        {
            if (_userId is null)
            {
                _userId = ResolveUserId();
            }

            return _userId.Value;
        }
    }

    private UserRoles ResolveUserRole()
    {
        if (Enum.TryParse<UserRoles>(Headers[XUserRole], out var roleFromHeader))
        {
            return roleFromHeader;
        }

        _tokenPayload ??= Headers.Authorization.ToString().GetPayload();

        return _tokenPayload.UserRole;
    }

    private Guid ResolveUserId()
    {
        if (Guid.TryParse(Headers[XUserId], out var userId))
        {
            return userId;
        }

        _tokenPayload ??= Headers.Authorization.ToString().GetPayload();

        return _tokenPayload.UserId;
    }
}