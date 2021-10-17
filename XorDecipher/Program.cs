using SecurityLabs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XorDecipher
{
    internal class Program
    {
        private static Dictionary<char, double> LetterFrequencies = new Dictionary<char, double>()
        {
            ['a'] = 0.082,
            ['b'] = 0.015,
            ['c'] = 0.028,
            ['d'] = 0.043,
            ['e'] = 0.13,
            ['f'] = 0.022,
            ['g'] = 0.02,
            ['h'] = 0.061,
            ['i'] = 0.07,
            ['j'] = 0.0015,
            ['k'] = 0.0077,
            ['l'] = 0.04,
            ['m'] = 0.024,
            ['n'] = 0.067,
            ['o'] = 0.075,
            ['p'] = 0.019,
            ['q'] = 0.00095,
            ['r'] = 0.06,
            ['s'] = 0.063,
            ['t'] = 0.091,
            ['u'] = 0.028,
            ['v'] = 0.0098,
            ['w'] = 0.024,
            ['x'] = 0.0015,
            ['y'] = 0.02,
            ['z'] = 0.00074
        };

        enum InputEncoding
        {
            PlainText,
            Base64,
            Hex
        }

        enum Operation
        {
            BruteforceSingleByteXor,
            BruteforceRepeatingXor,
            GetCharFrequencies
        }

        static void Main()
        {
            Console.WriteLine("Choose input encoding: \n0 - Plain text\n1 - Base64\n2 - HEX");
            var inputEncoding = (InputEncoding)Utils.GetInt("encoding", 2);

            Console.WriteLine($"Enter text to cipher/decipher: ");
            string input = Console.ReadLine();

            input = inputEncoding switch
            {
                InputEncoding.PlainText => input,
                InputEncoding.Base64 => Encoding.UTF8.GetString(Convert.FromBase64String(input)),
                InputEncoding.Hex => Encoding.UTF8.GetString(Convert.FromHexString(input)),
                _ => input
            };

            Console.WriteLine("Choose operation: \n0 - Bruteforce single-byte XOR\n1 - Bruteforce repeating-key XOR\n2 - Show character frequencies");
            var operation = (Operation)Utils.GetInt("operation", 2);

            switch (operation)
            {
                case Operation.BruteforceSingleByteXor:
                    BruteforceSingleByteXor(input);
                    break;
                case Operation.BruteforceRepeatingXor:
                    BruteforceRepeatingXor(input);
                    break;
                case Operation.GetCharFrequencies:
                    PrintCharFrequencies(input);
                    break;
            }

        }

        private static void BruteforceRepeatingXor(string input)
        {
            Console.WriteLine("Index of coincidence analysis");
            IndexOfCoincidenceAnalysis(input);

            int keyLength = Utils.GetInt("key length", input.Length, 1);

            var rows = input.Chunk(keyLength).ToArray();
            var columns = Enumerable
                .Range(0, keyLength)
                .Select(i => rows.Select(row => row
                        .ToCharArray()
                        .ElementAt(i))
                    .ToArray())
                .Select(columnLetters => new string(columnLetters));

            var columnsXorShifts = columns
                .Select(col => GetAllXorShifts(col)
                .OrderByDescending(shift =>
                    GetRelativeFrequencyCorelation(shift.Value)).ToArray())
                .ToArray();

            var textXorShifts = new Dictionary<string, string>();
            var bestKey = new string(columnsXorShifts.Select(shifts => shifts.First().Key).ToArray());
            Console.WriteLine($"Best key guess: {bestKey}");
            var decodedText = XorEncodeDecode(input, bestKey);
            Console.WriteLine($"Decoded text:\n { XorEncodeDecode(input, bestKey) }");
            Console.WriteLine("\nWrote to output.txt");
            File.WriteAllText("output.txt", XorEncodeDecode(input, bestKey));
        }

        private static string XorEncodeDecode(string text, string key)
        {
            var result = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                result.Append((char)(text[i] ^ key[i % key.Length]));
            }
            return result.ToString();
        }

        private static double GetRelativeFrequencyCorelation(string text)
        {
            // does not work properly?
            double relativeFrequencyCorelation = 0;
            foreach (var charFrequency in GetCharFrequencies(text).OrderBy(c => c.Value))
            {
                var letterOccurs = LetterFrequencies.TryGetValue(charFrequency.Key, out double letterFrequency);
                if (letterOccurs)
                {
                    relativeFrequencyCorelation += letterFrequency * charFrequency.Value;
                }
            }
            return relativeFrequencyCorelation;
        }

        private static Dictionary<char, double> GetCharFrequencies(string text)
        {
            return text.Distinct()
                .ToDictionary(c => c,
                    c => text
                         .Count(letter => letter == c)
                         / (double)text.Length);
        }

        private static void IndexOfCoincidenceAnalysis(string input)
        {
            for (int i = 1; i < input.Length; i++)
            {
                var shiftedText = input.Shift(i);
                int coincidenceCount = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    if (shiftedText[j] == input[j])
                    {
                        coincidenceCount++;
                    }
                }
                double indexOfCoincidence = (double)coincidenceCount / input.Length;
                Console.WriteLine($"Index of coincidence with shift = {i}: {indexOfCoincidence:F3}, {coincidenceCount} coincidences");
            }
        }

        private static string SingleByteXor(string input, char key)
        {
            return new string(input.ToCharArray().Select(c => (char)(c ^ key)).ToArray());
        }

        private static Dictionary<char, string> GetAllXorShifts(string input)
        {
            char[] keys = Enumerable.Range(0, 128)
                            .Select(i => (char)i)
                            .ToArray(); // ASCII table range
            Dictionary<char, string> output = new();
            foreach (var key in keys)
            {
                output[key] = SingleByteXor(input, key);
            }
            return output;
        }

        private static void BruteforceSingleByteXor(string input)
        {
            var output = GetAllXorShifts(input)
                .OrderByDescending(shift => GetRelativeFrequencyCorelation(shift.Value));
            var bestGuess = output.First();
            Console.WriteLine($"Best guess (key = {bestGuess.Key}): {bestGuess.Value}");
            File.WriteAllText("output.txt", string.Join('\n', output.Select(kvp => $"KEY {kvp.Key}: {kvp.Value}")));
            Console.WriteLine("\nWrote all variants to output.txt");
        }

        private static void PrintCharFrequencies(string input)
        {
            Dictionary<char, double> charFrequencies = GetCharFrequencies(input);
            Console.WriteLine("Character frequencies: ");
            foreach (var pair in charFrequencies.OrderBy(kvp => kvp.Value))
            {
                Console.WriteLine($"{pair.Key} --- {pair.Value}");
            }
            Console.WriteLine();
        }
    }
}
