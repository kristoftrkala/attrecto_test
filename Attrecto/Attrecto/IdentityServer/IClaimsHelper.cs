namespace Attrecto.IdentityServer
{
    public interface IClaimsHelper
    {
        public string GetRoleFromClaim(HttpContext httpContext);
        public int GetIdFromClaim(HttpContext httpContext);
    }
}
