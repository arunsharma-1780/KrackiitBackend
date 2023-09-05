using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OnlinePractice.API.Repository.Base
{
    internal class Autenticacion
    {
        private readonly JwtSecurityTokenHandler JwtTokenHandler;
        internal static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());


        public Autenticacion()
        {
            JwtTokenHandler = new JwtSecurityTokenHandler();
        }


        internal string GenerateJwtToken(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
            {
                throw new InvalidOperationException("Name is not specified.");
            }

            var claims = new[] { new Claim(ClaimTypes.Name, paramName) };
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("ExampleServer", "ExampleClients", claims, expires: DateTime.UtcNow.AddSeconds(60), signingCredentials: credentials);
            return JwtTokenHandler.WriteToken(token);
        }
    }
}
