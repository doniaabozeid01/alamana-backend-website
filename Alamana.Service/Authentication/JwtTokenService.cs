using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Alamana.Service.Authentication
{
    public class JwtTokenService : IJwtTokenService
    {

        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        //public string GenerateJwtToken(ApplicationUser user)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        //    var currentUtcDate = DateTime.UtcNow; // التأكد من استخدام نفس الوقت
        //    var expirationDate = DateTime.UtcNow.AddDays(30); // مدة التوكين ساعة بتوقيت UTC

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //    new Claim("FullName", user.FullName ?? string.Empty),
        //    new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
        //    new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"])
        //}),
        //        NotBefore = currentUtcDate, // إضافة NotBefore مع التوقيت نفسه
        //        Expires = expirationDate,
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}






        public string GenerateJwtToken(ApplicationUser user, string role)
        {
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var now = DateTime.UtcNow;
            var expires = now.AddDays(30);

            // جهزي الـclaims وتفادي null
            var claims = new List<Claim>
    {
        // اسم المعرّف
        new Claim(JwtRegisteredClaimNames.Sub,   user.Id ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier,     user.Id ?? string.Empty),

        // ايميل + اسم كامل
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new Claim("FullName",                    user.FullName ?? string.Empty),

        // دور المستخدم
        new Claim(ClaimTypes.Role,               role ?? string.Empty),

        // معلومات قياسية مفيدة
        new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat,   EpochSeconds(now), ClaimValueTypes.Integer64),
    };



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                Expires = expires,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256 // أو HmacSha256Signature
                )
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }






        // يحوّل وقت الإنبثاق إلى seconds since epoch
        static string EpochSeconds(DateTime utc)
        {
            var seconds = (long)Math.Floor((utc - DateTime.UnixEpoch).TotalSeconds);
            return seconds.ToString();
        }


    }
}
