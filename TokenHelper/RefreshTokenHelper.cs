﻿using ConverterRestApi.Data;
using ConverterRestApi.Helper;
using ConverterRestApi.Model;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace ConverterRestApi.TokenHelper
{
    public class RefreshTokenHelper : IRefreshToken
    {
        private readonly ConverterRestApiContext _context;
        
        public RefreshTokenHelper()
        {
        }

        public RefreshTokenHelper(ConverterRestApiContext context)
        {
            _context = context;
        }
        public (string RefreshToken, DateTime ExpirationTime) GenerateToken()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            string RefreshToken = Convert.ToBase64String(randomNumber);

            // Set the expiration time for the associated access token (e.g., 30 days from now)
            DateTime expirationTime = DateTime.UtcNow.Add(TimeSpan.FromDays(30));

            return (RefreshToken, expirationTime);
        }

        public string RefreshTokenGenerator(CredentialsParameters creds, LoginParameters userCred)
        {
            var (refreshToken, expiredTime) = GenerateToken();

            var users = _context.RefreshToken.FirstOrDefault(cred => cred.UserId == creds.UserName);
            if (users != null)
            {
                users.RefreshToken = refreshToken;
                users.ExpirationTime = expiredTime;
                _context.SaveChanges();
            }
            else
            {
                var newRefreshToken = new RefreshTokenParameters()
                {
                    UserId = userCred.UserName.ToLower(),
                    Password = userCred.Password.ToLower(),
                    Phone = creds.Phone,
                    Email = creds.Email.ToLower(),
                    TokenId = new Random().Next().ToString(),
                    RefreshToken = refreshToken,
                    IsActive = true,
                    ExpirationTime = expiredTime

                };
                _context.RefreshToken.Add(newRefreshToken);
                _context.SaveChanges();
            }

            return refreshToken;
        }
        public bool ValidateRefreshToken(CredentialsParameters creds)
        {
            var refreshTokenEntity = _context.RefreshToken.FirstOrDefault(refresh => refresh.UserId == creds.UserName && refresh.Phone == creds.Phone && refresh.Email == creds.Email);
            var currentTime = DateTime.Now;
            if (refreshTokenEntity != null)
            {
                DateTime expirationTime = refreshTokenEntity.ExpirationTime;
                TimeSpan difference = expirationTime - currentTime;
                var t = difference.TotalDays;

                return difference.TotalDays < 30;
            }

            return false;

        }

    }
}
