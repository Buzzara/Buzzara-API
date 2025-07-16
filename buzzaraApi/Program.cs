using buzzaraApi.Data;
using buzzaraApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using System.IdentityModel.Tokens.Jwt;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."))
    .EnableSensitiveDataLogging() // <-- mostra os valores dos parâmetros nas queries
    .LogTo(Console.WriteLine, LogLevel.Error) // <-- exibe os erros do EF no console
);

// Configuração de autenticação com JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(
    jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret Key is missing."));

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// JWT + cookies
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = JwtRegisteredClaimNames.NameId,
            RoleClaimType = "role",
            ClockSkew = TimeSpan.Zero, // segurança extra
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["accessToken"];
                if (!string.IsNullOrEmpty(token))
                    context.Token = token;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Registrar os seus serviços
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ServicoService>();
builder.Services.AddScoped<PerfilAcompanhanteService>();
//builder.Services.AddScoped<MidiaService>();
builder.Services.AddScoped<NovoUsuarioService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<DashboardService>();

builder.Services.AddHostedService<UsuarioStatusService>();

builder.Services.AddHttpClient<GeoNamesService>();

builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<QRCodeService>();

// Registrar o serviços de rotas publicas
builder.Services.AddScoped<PublicoService>();
builder.Services.AddScoped<AnuncioPublicoService>();



// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
          .WithOrigins(
             "http://localhost:8080",  // Novo frontend Vite
             "http://localhost:5173"   // seu painel React/Vite
          )
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
    });
});

// Configuração dos controllers e JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
