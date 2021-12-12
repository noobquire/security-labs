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
        private const int recordsCount = 100;
        static async Task Main(string[] args)
        {
            var passwordGenerator = new PasswordGenerator();
            var records = await passwordGenerator.GenerateRecordsAsync(recordsCount);
            PrintResult(records);
            SaveToCsv(await Task.Run(() => RunHashSha1(records)), "WeakPasswords");
            SaveToCsv(await Task.Run(() => RunHashArgon2i(records)), "StrongPasswords");
            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        static void PrintResult(IList<string> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }

        private static void SaveToCsv(List<string> records, string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var finalPath = Path.Combine(basePath, fileName + ".csv");
            using TextWriter sw = new StreamWriter(finalPath);

            foreach (var r in records)
            {
                sw.WriteLine(r);
            }
        }

        private static List<string> RunHashSha1(List<string> records)
        {
            var result = new List<string>();

            foreach (var record in records)
            {
                var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(record));
                result.Add(string.Concat(hash.Select(b => b.ToString("x2"))));
            }

            return result;
        }

        private static List<string> RunHashArgon2i(List<string> records)
        {
            var result = new List<string>();

            foreach (var record in records)
            {
                var stopwatch = Stopwatch.StartNew();

                Console.WriteLine($"Creating hash for password '{ record }'.");

                var salt = CreateSalt();
                Console.WriteLine($"Using salt '{ Convert.ToBase64String(salt) }'.");

                var hash = HashPassword(record, salt);
                Console.WriteLine($"Hash is '{ Convert.ToBase64String(hash) }'.");

                stopwatch.Stop();
                Console.WriteLine($"Process took { stopwatch.ElapsedMilliseconds / 1024.0 } s");

                result.Add($"Hash: {Convert.ToBase64String(hash)}, salt: {Convert.ToBase64String(salt)}");
            }

            return result;
        }

        private static byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // four cores
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(16);
        }

        private static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
