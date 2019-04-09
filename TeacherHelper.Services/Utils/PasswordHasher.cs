using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TeacherHelper.Services.Utils
{
    internal static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;

        internal static string HashPassword(string password)
        {
            // Generate salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Hash the password into bytes
            var hashHandler = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = hashHandler.GetBytes(HashSize);

            // Concat password hash and the hash salt
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);
            
            // Convert the hash + salt combination to base64 to store
            var hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword;
        }

        internal static bool CheckPassword(string hashedPassword, string password)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var hashHandler = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = hashHandler.GetBytes(HashSize);

            // Get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
