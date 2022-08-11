using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SharpStore.Entities;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SharpStore.Services
{
    public interface IJwtAuthService
    {
        Task<string> GetToken(string email, string id, List<string> roles);
    }

    public class JwtAuthService : IJwtAuthService
    {
        private readonly IJwtAuthSettings _settings;

        public JwtAuthService(IJwtAuthSettings settings)
        {
            _settings = settings;
        }

        public async Task<string> GetToken(string email, string id, List<string> roles)
        {
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, id),
                new("email", email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var rawToken = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(_settings.Key)),
                    SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(rawToken);
            return token;
        }
    }
}