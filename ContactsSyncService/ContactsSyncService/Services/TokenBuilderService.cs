using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ContactsSyncService.Services
{
    /// <summary>
    /// Создаёт токены.
    /// </summary>
    public class TokenBuilderService
    {
        private readonly IConfiguration _configuration;
        public TokenBuilderService(IConfiguration configuration) => _configuration = configuration;

        /// <summary>
        /// Создать токен без доп прав (без claims).
        /// </summary>
        /// <returns>Сгенерированный токен.</returns>
        public string GenerateToken()
        {
            var jwt = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AuthenticationSettings:Issuer"),
                audience: _configuration.GetValue<string>("AuthenticationSettings:Audience"),
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AuthenticationSettings:Key"))),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
