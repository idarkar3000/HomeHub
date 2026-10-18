using System.Security.Claims;

namespace HomeHub.Endpoints
{
    public static class ClaimsExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(value, out var id))
            {
                throw new UnauthorizedAccessException("Token sin identificador de usuario.");
            }

            return id;
        }
    }
}
