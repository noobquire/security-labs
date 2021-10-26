using SecurityLabs;
using System;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace PseudoRandomDecoder
{
    internal class Program
    {
        public static async Task Main()
        {
            var accountId = Utils.GetInt("account id");
            var numberOfValues = Utils.GetInt("number of values to retreive");
            using var fs = File.OpenWrite("values.txt");
            using var sw = new StreamWriter(fs);
            for (int i = 0; i < numberOfValues; i++)
            {
                var value = await GetMtValue(accountId);
                Console.WriteLine(value);
                sw.WriteLine(value);
            }
            Console.WriteLine($"Wrote {numberOfValues} pseudorandom values to values.txt");
        }

        private static int PredictLcg(int[] lcg)
        {
            //var modInverseA = ModInverse(lcg[0] - lcg[1], LcgRandom.Modulo);
            //var foundA = ((lcg[1] - lcg[2]) * modInverseA) % LcgRandom.Modulo;
            //var foundC = (lcg[2] - foundA * lcg[1]) % LcgRandom.Modulo;
            //Console.WriteLine($"found a = {foundA}, c = {foundC}");
            //var lcgRandom = new LcgRandom(lcg[2], foundA, foundC);
            //var next = lcgRandom.Next();
            //return next;
            return 42;
        }

        private static async Task<long> GetMtValue(int accountId)
        {
            var builder = new UriBuilder("http://95.217.177.249/casino/playBetterMt/");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["id"] = accountId.ToString();
            query["bet"] = "1";
            query["number"] = "1";

            builder.Query = query.ToString();
            var client = new HttpClient();
            client.BaseAddress = builder.Uri;


            var response = await client.GetAsync("");
            //response.EnsureSuccessStatusCode();
            var reponseString = await response.Content.ReadAsStringAsync();
            var casinoResponse = JsonSerializer.Deserialize<CasinoResponse>(reponseString);

            return casinoResponse.realNumber;
        }

        private static async Task<long> GetLcgValue(int accountId)
        {
            var builder = new UriBuilder("http://95.217.177.249/casino/playLcg/");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["id"] = accountId.ToString();
            query["bet"] = "1";
            query["number"] = "1";
            
            builder.Query = query.ToString();
            var client = new HttpClient();
            client.BaseAddress = builder.Uri;
            

            var response = await client.GetAsync("");
            //response.EnsureSuccessStatusCode();
            var reponseString = await response.Content.ReadAsStringAsync();
            var casinoResponse = JsonSerializer.Deserialize<CasinoResponse>(reponseString);

            return casinoResponse.realNumber;
        }

        public static BigInteger Egcd(BigInteger left,
                              BigInteger right,
                          out BigInteger leftFactor,
                          out BigInteger rightFactor)
        {
            leftFactor = 0;
            rightFactor = 1;
            BigInteger u = 1;
            BigInteger v = 0;
            BigInteger gcd = 0;

            while (left != 0)
            {
                BigInteger q = right / left;
                BigInteger r = right % left;

                BigInteger m = leftFactor - u * q;
                BigInteger n = rightFactor - v * q;

                right = left;
                left = r;
                leftFactor = u;
                rightFactor = v;
                u = m;
                v = n;

                gcd = right;
            }

            return gcd;
        }

        public static BigInteger ModInverse(BigInteger value, BigInteger modulo)
        {
            BigInteger x, y;
            Egcd(value, modulo, out x, out y);
            if (x < 0)
                x += modulo;

            return x % modulo;
        }
    }
}
