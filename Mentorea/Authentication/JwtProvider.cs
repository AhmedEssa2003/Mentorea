using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mentorea.Authentication
{
    public class JwtProvider(IConfiguration configuration) : IJwtProvider
    {
        private readonly IConfiguration _configuration = configuration;

        public (string Token, int ExpiredIn) GenrateToken(ApplicationUser user,string Role)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub,user.Id),    
                new(JwtRegisteredClaimNames.Email,user.Email!),    
                new(JwtRegisteredClaimNames.GivenName,user.Name),    
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(ClaimTypes.Role,Role)
                   
            ];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("Jwt:Issuer")!,
                audience: _configuration.GetValue<string>("Jwt:Audience")!,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3).AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryMinutes")!),
                signingCredentials: signingCredentials
            );
            return (Token: new JwtSecurityTokenHandler().WriteToken(token), ExpiredIn: _configuration.GetValue<int>("Jwt:ExpiryMinutes")!);
        }

        public string? ValidateToken(string Token)
        {
            try
            {
                var toenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")!));
                toenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false,
                }, out var securityToken);
                var jwtSecurityToken = (JwtSecurityToken)securityToken;
                return jwtSecurityToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
             
            }
            catch 
            {

                return null;
            }
        }
    }
}
