using System.Linq;

namespace CipherUtils
{
    public static class SingleXorCipher
    {
        public static string EncryptDecrypt(char key, string text)
        {
            return new string(text.ToCharArray().Select(c => (char)(c ^ key)).ToArray());
        }
    }
}
