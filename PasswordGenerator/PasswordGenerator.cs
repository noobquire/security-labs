using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    internal class PasswordGenerator
    {
        private readonly Random rnd = new();
        private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
        private static readonly string Numbers = "1234567890";

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
            var commonWord = Resources.CommonWords[rnd.Next(0, Resources.CommonWords.Length)];
            return ProcessCommonPassword(commonWord);

            string ProcessCommonPassword(string word)
            {
                if(rnd.Next(0, 20) > 17)
                {
                    word += Resources.CommonWords[rnd.Next(0, Resources.CommonWords.Length)];
                }

                var wordAsCharArray = word.ToCharArray();
                for (int i = 0; i < wordAsCharArray.Length; i++)
                {
                    switch (word[i])
                    {
                        case 'l':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '1' : 'l';
                            break;
                        case 'o':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '0' : 'o';
                            break;
                        case 'e':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '3' : 'e';
                            break;
                        case 'g':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '9' : 'g';
                            break;
                        case 't':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '2' : 't';
                            break;
                        case 'f':
                            wordAsCharArray[i] = rnd.Next(0, 3) > 1 ? '4' : 'f';
                            break;
                    }

                    if (char.IsLetter(wordAsCharArray[i]) && char.IsLower(wordAsCharArray[i]) && rnd.Next(0, 10) > 7)
                    {
                        wordAsCharArray[i] = char.ToUpper(wordAsCharArray[i]);
                    }
                }

                StringBuilder result = new(new string(wordAsCharArray));
                var countOfNumbersAfterPassword = rnd.Next(0, 6);
                for (int i = 0; i < countOfNumbersAfterPassword; i++)
                {
                    result.Append(Numbers[rnd.Next(0, Numbers.Length)]);
                }

                return result.ToString();
            }

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
