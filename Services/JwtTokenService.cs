using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UpdatingProjects.Services
{
    public class JwtTokenService
    {
        private IConfiguration _configuration { get; }
        private string _secKey { get; set; }
        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secKey = _configuration["TokenSecKey"];
        }

        public string GenerateToken(int id)
        {           
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secKey));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512);
            var header = new JwtHeader(credentials);
            var payload = new JwtPayload(id.ToString(), null, null, null, DateTime.Today.AddDays(1));
            var securityToken = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public JwtSecurityToken Verify(string jwtToken)
        {
            var tokenHendler = new JwtSecurityTokenHandler();
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secKey));
            tokenHendler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(hmac.Key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;           
        }
    }
}