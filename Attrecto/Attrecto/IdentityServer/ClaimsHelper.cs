using System.Security.Claims;

namespace Attrecto.IdentityServer
{
    public class ClaimsHelper : IClaimsHelper
    {
        public string GetRoleFromClaim(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return identity.Claims.FirstOrDefault(x=>x.Type.Contains("role")).Value;
            }
            return null;
        }

        public int GetIdFromClaim(HttpContext httpContext)
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
