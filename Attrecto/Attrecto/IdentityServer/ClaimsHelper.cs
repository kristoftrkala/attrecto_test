using System.Security.Claims;

namespace Attrecto.IdentityServer
{
    public static class ClaimsHelper
    {
        public static string GetRoleFromClaim(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return identity.Claims.FirstOrDefault(x=>x.Type.Contains("role")).Value;
            }
            return null;
        }

        public static int GetIdFromClaim(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return int.Parse(identity.Claims.FirstOrDefault(x => x.Type.Contains("nameidentifier")).Value);
            }
            return -1;
        }
    }
}
