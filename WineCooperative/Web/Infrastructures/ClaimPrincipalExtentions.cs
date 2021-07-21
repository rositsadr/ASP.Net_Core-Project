using System.Security.Claims;

namespace Web.Infrastructures
{
    public static class ClaimPrincipalExtentions
    {
        public static string GetId(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier).Value;
    }
}
