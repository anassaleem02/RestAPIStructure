
using System.Security.Cryptography;
using System.Text;

namespace CommonDataLayer.Helpers
{
    public static class Utilities
    {
        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = GenerateSalt();

            // Combine the password and salt
            byte[] saltedPassword = Combine(Encoding.UTF8.GetBytes(password), salt);

            // Hash the combined password and salt
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedPassword = sha256.ComputeHash(saltedPassword);

                // Combine hashed password and salt to store
                byte[] hashWithSalt = Combine(hashedPassword, salt);

                // Convert to base64 for storage
                return Convert.ToBase64String(hashWithSalt);
            }
        }

        // Generates a random salt
        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; // 16 bytes salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        // Combines two byte arrays
        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] combined = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combined, 0, first.Length);
            Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);
            return combined;
        }
    }
}
