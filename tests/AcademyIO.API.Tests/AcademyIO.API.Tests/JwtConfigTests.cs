using System.Text;
using AcademyIO.API.Configurations;
using AcademyIO.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;


namespace AcademyIO.API.Tests;
public class JwtConfigTests
{
    [Fact]
    public void AddJwt_ShouldConfigureJwtSettingsAndAuthentication()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {"AppSettings:SecretKey", "supersecretkey1234567890"},
            {"AppSettings:Issuer", "testIssuer"},
            {"AppSettings:Audience", "testAudience"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });
        builder.Configuration.AddConfiguration(configuration);

        // Act
        builder.AddJwt();

        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert

        // Verificar se JwtSettings está registrado e configurado
        var jwtSettings = serviceProvider.GetService<JwtSettings>();
        Assert.NotNull(jwtSettings);
        Assert.Equal("supersecretkey1234567890", jwtSettings.SecretKey);
        Assert.Equal("testIssuer", jwtSettings.Issuer);
        Assert.Equal("testAudience", jwtSettings.Audience);

        // Verificar autenticação registrada
        var authOptions = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var scheme = authOptions.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme).Result;
        Assert.NotNull(scheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, scheme.Name);

        // Verificar parâmetros da validação do token JWT
        var jwtBearerOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<JwtBearerOptions>>().Get(JwtBearerDefaults.AuthenticationScheme);
        Assert.True(jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey);
        Assert.Equal(jwtSettings.Issuer, jwtBearerOptions.TokenValidationParameters.ValidIssuer);
        Assert.Equal(jwtSettings.Audience, jwtBearerOptions.TokenValidationParameters.ValidAudience);

        var keyBytes = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
        var symmetricKey = jwtBearerOptions.TokenValidationParameters.IssuerSigningKey as SymmetricSecurityKey;
        Assert.NotNull(symmetricKey);
        Assert.Equal(keyBytes, symmetricKey.Key);
    }
}
