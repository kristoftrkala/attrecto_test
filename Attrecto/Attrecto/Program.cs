using Attrecto.Data;
using Attrecto.IdentityServer;
using Attrecto.Repositories;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AttrectoTestDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddIdentityServer()
                .AddDeveloperSigningCredential() //This is for dev only scenarios when you don’t have a certificate to use.
                .AddInMemoryApiResources(IdentityConfig.ApiResources)
                .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
                .AddInMemoryClients(IdentityConfig.Clients)
                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                .AddResourceOwnerValidator<PasswordValidator>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Audience = "attrecto_api";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtClaimTypes.Name,
            ValidateIssuerSigningKey = false,
            RequireSignedTokens = false,
            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
            {
                var jwt = new JwtSecurityToken(token);
                return jwt;
            },
            ValidateIssuer = false,
            ValidIssuers = new List<string>() { @"https://localhost:7226" },
            ValidateAudience = true,
            ValidAudience = "attrecto_api"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DefaultPolicy", options => options.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme));

    options.DefaultPolicy = options.GetPolicy("DefaultPolicy");
    options.InvokeHandlersAfterFailure = false;
});


builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
