using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.Utils
{
    public class PasswordUtils
    {
       
            private const char _segmentDelimiter = ':';

            public string HashSecret(string input)
            {
                var saltSize = 16;
                var iterations = 100_000;
                var keySize = 32;
                HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
                byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

                return string.Join(
                    _segmentDelimiter,
                    Convert.ToHexString(hash),
                    Convert.ToHexString(salt),
                    iterations,
                    algorithm
                );
            }

            public bool VerifyHash(string input, string hashString)
            {
                string[] segments = hashString.Split(_segmentDelimiter);
                byte[] hash =Convert.FromHexString(segments[0]);
                byte[] salt = Convert.FromHexString(segments[1]);
                int iterations = int.Parse(segments[2]);
                HashAlgorithmName algorithm = new(segments[3]);
                byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                    input,
                    salt,
                    iterations,
                    algorithm,
                    hash.Length
                );

                return CryptographicOperations.FixedTimeEquals(inputHash, hash);
            }
           
        }
    }
