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

            var values = new List<int>();
            for (int i = 0; i < numberOfValues; i++)
            {
                var value = await GetValue(accountId, mode);
                Console.WriteLine(value);
                values.Add(value);
            }

            var predictedValue = mode switch
            {
                Mode.Lcg => LcgRandom.Predict(values.ToArray()),
                Mode.Mt => MtRandom.BruteforceSeed(values.First()),
                Mode.BetterMt => MtRandom.Predict(values.ToArray()),
                _ => 42
            };

            Console.WriteLine($"predicted next value: {predictedValue}");
            var response = await TryValue(predictedValue, accountId, mode);
            if (response.realNumber == predictedValue)
            {
                Console.WriteLine("Cracked successfully!");
            }
            else
            {
                Console.WriteLine("Failed to crack :(");
            }
        }

        private static async Task<int> GetValue(string accountId, Mode mode)
        {
            var response = await TryValue(1, accountId, mode);
            return response.realNumber;
        }

        /// <summary>
        /// Tries to create account
        /// </summary>
        /// <returns>Account id</returns>
        private static async Task<string> CreateAccount()
        {
            int id = new Random().Next(0, 100000);
            while (true)
            {
                var url = $"http://95.217.177.249/casino/createacc?id={id}";
                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var account = JsonSerializer.Deserialize<Account>(responseString);
                    Console.WriteLine($"created account with id = {account.id}");
                    return account.id;
                }

                Console.WriteLine($"account {id} already exists, retrying");
                id = new Random().Next(0, 100000);
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
