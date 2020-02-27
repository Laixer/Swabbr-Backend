using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Swabbr.WowzaStreamingCloud.Services
{


    public static class WowzaAuthenticationService
    {

        private const double tokenMinutesValid = 25;
        private const string tokenIssuer = "Swabbr";

        private static WowzaHmacToken GenerateToken(string sharedKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sharedKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(tokenMinutesValid);

            var token = new JwtSecurityToken(
                tokenIssuer,
                expires: expires,
                signingCredentials: credentials
                // Stream id
            );

            return new WowzaHmacToken
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expires
            };
        }

        public static WowzaHmacToken GenerateTokenHmac(string sharedKey)
        {
            var token = GenerateToken(sharedKey);
            var encoding = new ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(sharedKey);
            byte[] messageBytes = encoding.GetBytes(token.JwtToken);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                //token.Hmac = Convert.ToBase64String(hashmessage);
                token.Hmac = ToHexString(hashmessage);
            }

            return token;
        }

        private static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

    }
}
