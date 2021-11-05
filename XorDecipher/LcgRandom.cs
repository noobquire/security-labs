namespace CipherUtils
{
    public class LcgRandom
    {
        private long _last;
        private readonly int a, c;

        public LcgRandom(int first, int a, int c)
        {
            _last = first;
            (this.a, this.c) = (a, c);
        }

        public int Next()
        {
            _last = (a * _last + c) % uint.MaxValue;
            return (int) _last;
        }
    }
}
