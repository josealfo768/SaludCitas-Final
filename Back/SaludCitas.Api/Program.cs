using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SaludCitas.Api.Middleware;
using SaludCitas.Application;
using SaludCitas.Infrastructure;
using SaludCitas.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Configuración leída de appsettings.json
var jwt = builder.Configuration.GetSection(JwtSettings.Seccion).Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la sección Jwt en appsettings.json");
var cadenaConexion = builder.Configuration.GetConnectionString("SaludCitas")
    ?? throw new InvalidOperationException("Falta la cadena de conexión SaludCitas");

// Capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(cadenaConexion, jwt);

// API
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Autenticación: valida el token JWT que envía el cliente
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Emisor,
            ValidateAudience = true,
            ValidAudience = jwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "role",
            NameClaimType = "sub"
        };
    });
builder.Services.AddAuthorization();

// CORS: permite que Angular (localhost:4200) llame a esta API
var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? [];
builder.Services.AddCors(opciones =>
    opciones.AddDefaultPolicy(politica =>
        politica.WithOrigins(origenes).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();