using System;
using System.Collections.Generic;

namespace PseudoRandomDecoder
{
    public class MtRandom
    {
        private static readonly int w = 32;
        private static readonly int n = 624;
        private static readonly int f = 1812433253;
        static readonly int m = 397, r = 31;
        static readonly uint a = 0x9908B0DF;
        static readonly uint d = 0xFFFFFFFF, b = 0x9D2C5680, c = 0xEFC60000;
        static readonly int u = 11, s = 7, t = 15, l = 18;
        static readonly long lower_mask = (1 << r) - 1L;
        static readonly long upper_mask = ~lower_mask;

        private uint[] List { get; set; }
        private int IndexOfList { get; set; }

        public MtRandom() 
        {
            List = new uint[n];

            List[0] = (uint)new Random().Next(0);
            IndexOfList = n;
            for (int i = 1; i < n; i++)
            {
                List[i] = (uint)(f * (List[i - 1] ^ (List[i - 1] >> (w - 2))) + i);
            }
        }

        public MtRandom(uint seed)
        {
            List = new uint[n];
            List[0] = seed;
            IndexOfList = n;
            for (int i = 1; i < n; i++)
            {
                List[i] = (uint)(f * (List[i - 1] ^ (List[i - 1] >> (w - 2))) + i);
            }
        }

        public static int BruteforceSeed(long firstMtValue)
        {
            var seed = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var mtInstance = new MtRandom(seed - 1);

            for (uint i = 0; i <= 10; i++)
            {
                seed += i;
                mtInstance = new MtRandom(seed);

                if (mtInstance.GetValue() == firstMtValue)
                {
                    Console.WriteLine($"Seed: {seed}");
                    break;
                }
            }

            return (int)mtInstance.GetValue();
        }

        public static long PredictHardSeededMt(long[] mtValues)
        {
            var mtGenerator = new MtRandom
            {
                List = RecreateStates(mtValues)
            };

            return mtGenerator.GetValue();
        }

        public uint GetValue()
        {
            if (IndexOfList > n)
            {
                throw new ArgumentException("More then could be");
            }
            else if (IndexOfList == n)
            {
                Twist();
            }

            var result = Temper(List[IndexOfList]);
            IndexOfList++;
            return result;
        }

        private static uint Temper(uint x)
        {
            uint y = x;
            y ^= (y >> u) & d;
            y ^= (y << s) & b;
            y ^= (y << t) & c;
            y ^= y >> l;

            return y;
        }

        private static uint[] RecreateStates(long[] input)
        { 
            var result = new List<uint>();

            foreach (long item in input)
            {
                result.Add(UnTemper((uint)item));
            }

            return result.ToArray();
        }

        private static uint UnTemper(uint v)
        {
            var mask = 0x7F;

            var y = v ^ (v >> l);
            y ^= (y << t) & c;

            for (int i = 0; i < 4; i++)
            {
                uint newB = (uint)(b & ((mask) << (s * (i + 1))));
                y ^= (y << s) & newB;
            }

            for (int i = 0; i < 3; i++)
            {
                y ^= y >> u;
            }

            return y;
        }

        private void Twist()
        {
            uint temp;
            uint tempA;
            for (int i = 0; i < n; i++)
            {
                temp = (uint)((List[i] & upper_mask) + (List[(i + 1) % n] & lower_mask));
                tempA = temp >> 1;
                if (temp % 2 != 0)
                {
                    tempA ^= a;
                }
                List[i] = List[(i + m) % n] ^ tempA;
            }
            IndexOfList = 0;
        }
    }
}
