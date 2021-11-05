using System.Text;

namespace CipherUtils
{
    public static class RepeatingXorCipher
    {
        public static string EncryptDecrypt(string key, string text)
        {
            var result = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                result.Append((char)(text[i] ^ key[i % key.Length]));
            }
            return result.ToString();
        }
    }
}
