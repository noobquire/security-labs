using SecurityLabs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CipherUtils
{
    internal class Program
    {
        enum InputEncoding
        {
            PlainText,
            Base64,
            Hex,
            Binary
        }

        enum Operation
        {
            BruteforceSingleByteXor,
            BruteforceRepeatingXor,
            GetCharFrequencies,
            ShowText
        }

        static void Main()
        {
            while (true)
            {


                Console.WriteLine("Choose input encoding: \n0 - Plain text\n1 - Base64\n2 - HEX\n3 - Binary");
                var inputEncoding = (InputEncoding)Utils.GetInt("encoding", 3);

                Console.WriteLine($"Enter text to cipher/decipher: ");
                Console.SetIn(new StreamReader(Console.OpenStandardInput(),
                               Console.InputEncoding,
                               false,
                               bufferSize: 100000));
                string input = Console.ReadLine();

                input = inputEncoding switch
                {
                    InputEncoding.PlainText => input.ToNormalizedPlaintext(),
                    InputEncoding.Base64 => Encoding.ASCII.GetString(Convert.FromBase64String(input)),
                    InputEncoding.Hex => Encoding.ASCII.GetString(Convert.FromHexString(input)),
                    InputEncoding.Binary => Encoding.ASCII.GetString(stringToBytes(input)),
                    _ => input
                };

                byte[] stringToBytes(string binary) => binary.Chunk(8).Select(s => Convert.ToByte(s, 2)).ToArray();

                Console.WriteLine("Choose operation: \n0 - Bruteforce single-byte XOR\n1 - Bruteforce repeating-key XOR\n2 - Show character frequencies\n3 - Show text");
                var operation = (Operation)Utils.GetInt("operation", 3);

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
                    case Operation.ShowText:
                        ShowText(input);
                        break;
                }

                Console.WriteLine("Press any key for another operation, Ctrl + C to exit...");
                Console.ReadKey();
            }
        }

        private static void ShowText(string input)
        {
            Console.WriteLine(input);
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
                    FrequencyAnalysis.GetLetterOverallFrequency(shift.Value)).ToArray())
                .ToArray();

            var textXorShifts = new Dictionary<string, string>();

            var bestKey = new string(columnsXorShifts.Select(shifts => shifts.First().Key).ToArray());
            Console.WriteLine($"Best key guess: {bestKey}");
            var decodedText = RepeatingXorCipher.EncryptDecrypt(bestKey, input);
            Console.WriteLine($"Decoded text:\n {decodedText}");
            Console.WriteLine("\nWrote to output.txt");
            File.WriteAllText("output.txt", decodedText);
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

        private static Dictionary<char, string> GetAllXorShifts(string input)
        {
            char[] keys = Enumerable.Range(0, 128)
                            .Select(i => (char)i)
                            .ToArray(); // ASCII table range
            Dictionary<char, string> output = new();
            foreach (var key in keys)
            {
                if(!char.IsLetterOrDigit(key))
                {
                    continue;
                }
                output[key] = SingleXorCipher.EncryptDecrypt(key, input);
            }
            return output;
        }

        private static void BruteforceSingleByteXor(string input)
        {
            var output = GetAllXorShifts(input)
                .OrderByDescending(shift => FrequencyAnalysis.GetLetterOverallFrequency(shift.Value));
            var bestGuess = output.First();
            Console.WriteLine($"Best guess (key = {bestGuess.Key}): {bestGuess.Value}");
            File.WriteAllText("output.txt", string.Join('\n', output.Select(kvp => $"KEY {kvp.Key}: {kvp.Value}")));
            Console.WriteLine("\nWrote all variants to output.txt");
        }

        private static void PrintCharFrequencies(string input)
        {
            var charFrequencies = FrequencyAnalysis.GetCharFrequencies(input);
            Console.WriteLine("Character frequencies: ");
            foreach (var pair in charFrequencies.OrderBy(kvp => kvp.Value))
            {
                Console.WriteLine($"{pair.Key} --- {pair.Value}");
            }
            Console.WriteLine();
        }
    }
}
