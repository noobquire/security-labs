namespace PseudoRandomDecoder
{
    public class LcgRandom
    {
        public const long Modulo = 4294967296;
        private long _last;
        private readonly long a, c;

        public LcgRandom(int first, long a, long c)
        {
            _last = first;
            (this.a, this.c) = (a, c);
        }

        public int Next()
        {
            _last = (a * _last + c) % Modulo;
            return (int)_last;
        }
    }
}
