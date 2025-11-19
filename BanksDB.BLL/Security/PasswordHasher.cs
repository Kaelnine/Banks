using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Security
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 50000;
        
        public static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(KeySize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string hashString)
        {
            var parts = hashString.Split('.');
            if (parts.Length == 2)
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] hash = Convert.FromBase64String(parts[1]);                
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                var computedHash = pbkdf2.GetBytes(32); 

                return computedHash.SequenceEqual(hash);
            }
            else if (parts.Length == 3)
            {
                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var computedHash = pbkdf2.GetBytes(32);

                return computedHash.SequenceEqual(hash);
            }
            else
            {
                return false;
            }
            //if (parts.Length != 3)
            //{
            //    return false;
            //}
            //int iterations = int.Parse(parts[0]);
            //byte[] salt = Convert.FromBase64String(parts[1]);
            //byte[] hash = Convert.FromBase64String(parts[2]);
            //using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            //var computedHash = pbkdf2.GetBytes(KeySize);
            //return computedHash.SequenceEqual(hash);
        }
    }
}
