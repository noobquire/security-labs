using SecurityLabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace PseudoRandomDecoder
{
    internal class Program
    {
        enum Mode
        {
            Lcg,
            Mt,
            BetterMt
        }

        public static async Task Main()
        {
            var accountId = await CreateAccount();
            var mode = (Mode)Utils.GetInt("mode (0 = LCG, 1 = MT, 2 = MT with strong seed)", 2, 0);

            var numberOfValues = mode switch
            {
                Mode.Lcg => 3,
                Mode.Mt => 1,
                Mode.BetterMt => 624,
                _ => 1
            };

            var values = mode switch
            {
                Mode.Lcg => await GetCorrectLcgValues(numberOfValues, accountId, mode),
                Mode.Mt => await GetCorrectMtValues(numberOfValues, accountId, mode),
                Mode.BetterMt => await GetCorrectMtValues(numberOfValues, accountId, mode),
                _ => default
            };

            var predictedValue = mode switch
            {
                Mode.Lcg => LcgRandom.Predict(values),
                Mode.Mt => MtRandom.BruteforceSeed(values.First()),
                Mode.BetterMt => MtRandom.PredictHardSeededMt(values),
                _ => 42
            };

            Console.WriteLine($"Predicted next value: {predictedValue}");
            var response = await TryValue(predictedValue, accountId, mode);
            if (response.RealNumber == predictedValue)
            {
                Console.WriteLine("Cracked successfully!");
                Console.WriteLine(response.Message);
            }
            else
            {
                Console.WriteLine("Failed to crack :(");
            }

            Console.ReadKey();
        }

        private static async Task<long[]> GetCorrectLcgValues(int numberOfValues, string accountId, Mode mode)
        {
            var values = new long[numberOfValues];
            long helper;

            do
            {
                for (int i = 0; i < numberOfValues; i++)
                {
                    var value = await GetValue(accountId, mode);
                    Console.WriteLine(value);
                    values[i] = value;
                }

            } while (!LcgRandom.TryModulusInverse(values[0] - values[1], (long)Math.Pow(2, 32), out helper));

            return values;
        }

        private static async Task<long[]> GetCorrectMtValues(int numberOfValues, string accountId, Mode mode)
        {
            var values = new long[numberOfValues];

            for (int i = 0; i < numberOfValues; i++)
            {
                var value = await GetValue(accountId, mode);
                Console.WriteLine(value);
                values[i] = value;
            }

            return values;
        }

        private static async Task<long> GetValue(string accountId, Mode mode)
        {
            var response = await TryValue(1, accountId, mode);

            while (response is null)
            {
                response = await TryValue(1, accountId, mode);
            }

            return response.RealNumber;
        }

        /// <summary>
        /// Tries to create account
        /// </summary>
        /// <returns>Account id</returns>
        private static async Task<string> CreateAccount()
        {
            var id = Guid.NewGuid();
            while (true)
            {
                var url = $"http://95.217.177.249/casino/createacc?id={id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var account = JsonSerializer.Deserialize<Account>(responseString);
                    Console.WriteLine($"Created account with id = {account.Id}");
                    return account.Id;
                }

                Console.WriteLine($"Account {id} already exists, retrying");
                id = Guid.NewGuid();
            }
        }

        private static async Task<CasinoResponse> TryValue(int value, string accountId, Mode mode)
        {
            var builder = new UriBuilder($"http://95.217.177.249/casino/play{mode}/");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["id"] = accountId.ToString();
            query["bet"] = "1";
            query["number"] = value.ToString();

            builder.Query = query.ToString();
            var client = new HttpClient();
            client.BaseAddress = builder.Uri;

            var response = await client.GetAsync("");
            response.EnsureSuccessStatusCode();
            var reponseString = await response.Content.ReadAsStringAsync();
            var casinoResponse = JsonSerializer.Deserialize<CasinoResponse>(reponseString);

            return casinoResponse;
        }
    }
}
