using System;

namespace PseudoRandomDecoder
{
    public class LcgRandom
    {
        private const long Modulus = 4294967296;

        private static long _last;
        private static long a, c;

        public static int Next()
        {
            _last = (a * _last + c) % Modulus;
            return (int)_last;
        }

        public static int Predict(long[] lcgValues)
        {
            long modulusInverse;
            TryModulusInverse(lcgValues[0] - lcgValues[1], Modulus, out modulusInverse);

            a = (lcgValues[1] - lcgValues[2]) * modulusInverse % Modulus;
            c = (lcgValues[1] - a * lcgValues[0]) % Modulus;

            _last = lcgValues[2];
            return Next();
        }

        public static bool TryModulusInverse(long number, long modulo, out long result)
        {
            if (number < 1 || modulo < 2)
            {
                result = default;
                return false;
            }

            long n = number;
            long i = modulo, v = 0, d = 1;
            while (n > 0)
            {
                long t = i / n, x = n;
                n = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= modulo;
            if (v < 0) v = (v + modulo) % modulo;

            if (v * number % modulo == 1)
            {
                result = v;
                return true;
            }

            result = default;
            return false;
        }
    }
}
