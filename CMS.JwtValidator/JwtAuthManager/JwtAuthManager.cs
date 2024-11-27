using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace CMS.JwtGenerator
{
    public class JwtAuthManager : IJwtAuthManager
    {
        JwtTokenConfig jwtTokenConfig;
        public JwtAuthManager(JwtTokenConfig _jwtTokenConfig)
        {
            jwtTokenConfig = _jwtTokenConfig;
        }
        private static string jwtSecret = string.Empty;
        private byte[] _secret = Encoding.ASCII.GetBytes(jwtSecret);


        public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now)
        {
            _secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: now.AddMinutes(jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                ExpireAt=DateTime.Now.AddMinutes(jwtTokenConfig.AccessTokenExpiration)
            };
        }

        
        public (ClaimsPrincipal, JwtSecurityToken?) DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtTokenConfig.Issuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(_secret),
                        ValidAudience = jwtTokenConfig.Audience,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    },
                    out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }

       
    }

    public class JwtAuthResult
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; } = string.Empty;
        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }

    
}
