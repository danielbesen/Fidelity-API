using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Fidelity.Models
{
    public class Encrypt
    {
        public static string EncryptPass(string Password)
        {
            string encrypted;

            using (SHA256 sha256Hash = SHA256.Create())
            {
                encrypted = GetHash(sha256Hash, Password);
            }

            return encrypted;
        }

        public static bool VerifyPass(string Password, string EncryptedPass)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                if (VerifyHash(sha256Hash, Password, EncryptedPass))
                    return true;
                else
                    return false;
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            var hashOfInput = GetHash(hashAlgorithm, input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}