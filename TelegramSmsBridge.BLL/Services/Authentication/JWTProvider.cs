using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TelegramSmsBridge.BLL.Models;

namespace TelegramSmsBridge.BLL.Services.Authentification;

public class JWTProvider : IJWTProvider
{
    private readonly JWTSettings _jwtSettings;

    public JWTProvider(JWTSettings jWTSettings)
    {
        _jwtSettings = jWTSettings;
    }

    public string GenerateAccesstoken(string telegramUserName)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, telegramUserName)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryTime)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}