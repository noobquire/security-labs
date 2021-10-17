using SecurityLabs;
using System;
using System.Net.Http;

namespace PseudoRandomDecoder
{
    internal class Program
    {
        private static HttpClient httpClient;
        private static int _last;

        static void Main(string[] args)
        {
            while(true)
            {
                var a = DateTime.Parse("2021-10-17T08:35:55.797482Z");
                var c = 4;
                _last = Utils.GetInt("last lcg number", int.MaxValue, int.MinValue);
                Console.WriteLine(Lcg(GetUnixEpoch(a), c));
            }
        }

        private static int Lcg(int a, int c)
        {
            _last = (a * _last + c) % int.MaxValue;
            return _last;
        }

        public static int GetUnixEpoch(DateTime dateTime)
        {
            var unixTime = dateTime.ToUniversalTime() -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (int)unixTime.TotalSeconds;
        }
    }
}
