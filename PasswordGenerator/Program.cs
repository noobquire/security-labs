using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasswordGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var passwordGenerator = new PasswordGenerator();
            PrintResult(await passwordGenerator.GenerateRecordsAsync(1000));
            Console.ReadKey();
        }

        static void PrintResult(IList<string> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }
    }
}
