using BusinessLib.Contact;
using CMS.JwtGenerator;
using Contact_Management_Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtTokenConfig = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>()!;
IServiceCollection services = new ServiceCollection();
builder.Services.AddSingleton(jwtTokenConfig);
builder.Services.AddSingleton<ContactJsonFile>();
builder.Services.AddSingleton<AccountLoginFile>();
builder.Services.AddSingleton<IJwtAuthManager,JwtAuthManager>();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
        ValidAudience = jwtTokenConfig.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
// Add services to the container.
//services.AddJwtAuth();
builder.Services.AddControllers();

builder.Services.AddScoped<IContactService, ContactService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<ContactJsonFile>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lowercase
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {securityScheme, Array.Empty<string>()}
            });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CMS",
        builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});


var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CMS API V1");
        c.DocumentTitle = "CMS API";
    });
}
app.UseCors("CMS");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
