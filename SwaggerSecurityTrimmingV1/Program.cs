using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwaggerSecurityTrimmingV1.FilterSwagger;
using SwaggerSecurityTrimmingV1.Models;
using SwaggerSecurityTrimmingV1.Models.Entities;
using SwaggerSecurityTrimmingV1.Repositories.RoleRepository;
using SwaggerSecurityTrimmingV1.Repositories.UserRepositories;
using SwaggerSecurityTrimmingV1.Services.Authenticators;
using SwaggerSecurityTrimmingV1.Services.TokenGenerators;
using System.Text;

AuthenticationConfiguration _authenticationConfiguration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

_authenticationConfiguration = new();
builder.Configuration.Bind("Authentication", _authenticationConfiguration);

builder.Services.AddSingleton(_authenticationConfiguration);
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddScoped<AccessTokenGenerator>();
builder.Services.AddScoped<Authenticator>();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfiguration.Secret));

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = signingKey,
        ValidIssuer = _authenticationConfiguration.Issuer,
        ValidAudience = _authenticationConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options => options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build()).AddHttpContextAccessor();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.DocumentFilter<SecurityTrimming>();
    opt.OperationFilter<AllowAnonymousAuthentication_OperationsFilter>();
    opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."

    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = JwtBearerDefaults.AuthenticationScheme
                              }
                          },
                         new string[] {}
                    }
                });
});

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.RequireAuthenticationOn
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectJavascript("/swagger-custom.js");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

IUserRepository userRepo = app.Services.GetService<IUserRepository>();
User admin = new User() { UserName = "Admin", Password = "1234!", Active = true, Email = "admin@email.de" };
admin.AddRole("Admin");
admin.AddRole("Admin2");
User normalUser = new User() { UserName = "User", Password = "1234", Active = true, Email = "user@email.de" };
normalUser.AddRole("User");
userRepo.Create(admin);
userRepo.Create(normalUser);

app.Run();
