using System.Security.Claims;
using static Web.WebConstants;

namespace Web.Infrastructures
{
    public static class ClaimPrincipalExtentions
    {
        public static string GetId(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(AdministratorRole);

        public static bool IsMember(this ClaimsPrincipal user) => user.IsInRole(MemberRole);
    }
}
