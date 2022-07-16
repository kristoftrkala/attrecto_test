using Attrecto.Data;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Attrecto.IdentityServer
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly AttrectoTestDbContext dbContext;

        public PasswordValidator(AttrectoTestDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await dbContext.Users.Include(x => x.FkRoleNavigation)
                .SingleOrDefaultAsync(u => u.Email == context.UserName);

            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(context.Password, user.Password))
                {
                    context.Result = new GrantValidationResult(
                        subject: user.IdUser.ToString(),
                        authenticationMethod: "custom",
                        claims: new Claim[]
                        {
                            new Claim(JwtClaimTypes.Email, user.Email),
                            new Claim("role", user.FkRoleNavigation.Name)
                        });

                    return;
                }
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username/password");
            throw new UnauthorizedAccessException("Invalid username/password");
        }
    }
}
