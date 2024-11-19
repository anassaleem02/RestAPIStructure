
using System.Security.Cryptography;
using System.Text;

namespace CommonDataLayer.Helpers
{
    public static class Utilities
    {
        public static (string, string) HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = GenerateSalt();

            // Hash the password with PBKDF2 using the generated salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000)) // Adjust iteration count as needed
            {
                byte[] hashedPassword = pbkdf2.GetBytes(32); // Assuming you want a 32-byte hash

                // Convert both the hashed password and salt to base64 for storage
                return (Convert.ToBase64String(hashedPassword), Convert.ToBase64String(salt));
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
        private static string HashPasswordWithSalt(string password, string saltBase64)
        {
            // Convert the base64 salt back to byte array
            byte[] salt = Convert.FromBase64String(saltBase64);

            // Hash the password with PBKDF2 using the provided salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000)) // Use the same iteration count
            {
                byte[] hashedPassword = pbkdf2.GetBytes(32); // Assuming you want a 32-byte hash
                return Convert.ToBase64String(hashedPassword); // Return as base64 string
            }
        }
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            // Generate a hash from the entered password and the stored salt
            string hashedEnteredPassword = HashPasswordWithSalt(enteredPassword, storedSalt);

            // Compare the hashed password with the stored hash
            return hashedEnteredPassword == storedHash;
        }
    }
}
