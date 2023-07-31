using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shop.Models;
using Microsoft.IdentityModel.Tokens;

namespace Shop.Services
{
  public static class TokenService
  {
    public static string GenerateToken(User user)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      byte[] key = Encoding.ASCII.GetBytes(s: Settings.Secret);

      SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
      {

        Subject = new ClaimsIdentity(claims: new Claim[] {
           new Claim(type: ClaimTypes.Name, value: user.Username.ToString()), new Claim(type: ClaimTypes.Role, value: user.Role.ToString())
            }),
        Expires = DateTime.UtcNow.AddHours(value: 2),

        SigningCredentials = new SigningCredentials(key: new SymmetricSecurityKey(key: key), algorithm: SecurityAlgorithms.HmacSha256Signature)
      };
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor: tokenDescriptor);
      return tokenHandler.WriteToken(token: token);


    }

  }
}