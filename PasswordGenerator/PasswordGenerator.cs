using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    internal class PasswordGenerator
    {
        private readonly Random rnd = new Random();
        private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        internal Task<List<string>> GenerateRecordsAsync(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("count");
            }

            return GenerateRecordsInternalAsync(count);
        }

        private async Task<List<string>> GenerateRecordsInternalAsync(int count)
        {
            List<string> result = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                switch (rnd.Next(0, 11))
                {
                    case 1:
                        result.Add(await Task.Run(() => CreateCommonPassword()));
                        break;
                    case 2:
                        result.Add(await Task.Run(() => CreateStrongPassword()));
                        break;
                    default:
                        result.Add(await Task.Run(() => CreateUsualPassword()));
                        break;
                }
            }

            return result;
        }

        private string CreateCommonPassword()
        {
            return Resources.CommonPasswords[rnd.Next(0, Resources.CommonPasswords.Length)];
        }

        private string CreateUsualPassword()
        {
            var length = rnd.Next(5, 15);
            StringBuilder res = new StringBuilder();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        private string CreateStrongPassword()
        {
            var length = rnd.Next(20, 31);
            var numberOfNonAlphanumericCharacters = rnd.Next(5, length - 15);

            using var rng = RandomNumberGenerator.Create();
            var byteBuffer = new byte[length];

            rng.GetBytes(byteBuffer);

            var count = 0;
            var characterBuffer = new char[length];

            for (var iter = 0; iter < length; iter++)
            {
                var i = byteBuffer[iter] % 87;

                if (i < 10)
                {
                    characterBuffer[iter] = (char)('0' + i);
                }
                else if (i < 36)
                {
                    characterBuffer[iter] = (char)('A' + i - 10);
                }
                else if (i < 62)
                {
                    characterBuffer[iter] = (char)('a' + i - 36);
                }
                else
                {
                    characterBuffer[iter] = Punctuations[i - 62];
                    count++;
                }
            }

            if (count >= numberOfNonAlphanumericCharacters)
            {
                return new string(characterBuffer);
            }

            for (int j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
            {
                int k;
                do
                {
                    k = rnd.Next(0, length);
                }
                while (!char.IsLetterOrDigit(characterBuffer[k]));

                characterBuffer[k] = Punctuations[rnd.Next(0, Punctuations.Length)];
            }

            return new string(characterBuffer);
        }
    }
}
