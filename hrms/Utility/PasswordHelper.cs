using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace hrms.Utility
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt,
                KeyDerivationPrf.HMACSHA256,
                10000,
                256 / 8));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }
        public static bool Verify(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            var salt = Convert.FromBase64String(parts[0]);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password, salt,
                KeyDerivationPrf.HMACSHA256,
                10000,
                256 / 8));

            return hashed == parts[1];
        }

        internal static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-+";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
