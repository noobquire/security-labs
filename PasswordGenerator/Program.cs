using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    class Program
    {
        private const int recordsCount = 100000;
        static async Task Main(string[] args)
        {
            var passwordGenerator = new PasswordGenerator();
            var records = await passwordGenerator.GenerateRecordsAsync(recordsCount);
            PrintResult(records);
            SaveToCsv(records, "WeakPasswords", SHA1.Create());
            //SaveToCsv(records, "StrongPasswords");
            Console.ReadKey();
        }

        static void PrintResult(IList<string> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }

        private static void SaveToCsv(List<string> records, string fileName, HashAlgorithm hashAlgorithm)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var finalPath = Path.Combine(basePath, fileName + ".csv");
            using TextWriter sw = new StreamWriter(finalPath);

            foreach (var r in records)
            {
                var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(r));
                sw.WriteLine(string.Concat(hash.Select(b => b.ToString("x2"))));
            }
        }

        public void Run()
        {
            var password = "Hello World!";
            var stopwatch = Stopwatch.StartNew();

            Console.WriteLine($"Creating hash for password '{ password }'.");

            var salt = CreateSalt();
            Console.WriteLine($"Using salt '{ Convert.ToBase64String(salt) }'.");

            var hash = HashPassword(password, salt);
            Console.WriteLine($"Hash is '{ Convert.ToBase64String(hash) }'.");

            stopwatch.Stop();
            Console.WriteLine($"Process took { stopwatch.ElapsedMilliseconds / 1024.0 } s");

            stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Verifying hash...");

            var success = VerifyHash(password, salt, hash);
            Console.WriteLine(success ? "Success!" : "Failure!");

            stopwatch.Stop();
            Console.WriteLine($"Process took { stopwatch.ElapsedMilliseconds / 1024.0 } s");
        }

        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // four cores
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(16);
        }

        private bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
