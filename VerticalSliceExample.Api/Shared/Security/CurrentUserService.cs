using System.Security.Claims;

namespace VerticalSliceExample.Api.Shared.Security;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)  : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            if (Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}