using System.Text;

namespace CipherUtils
{
    public static class PolySubstitutionCipher
    {
        private const string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        public static string Encrypt(string[] alphabets, string plainText)
        {
            var cipherTextBuilder = new StringBuilder();
            for (int i = 0; i < plainText.Length; i++)
            {
                var alphabetNumber = i % alphabets.Length;
                string currentAlphabet = alphabets[alphabetNumber];
                var plainTextChar = plainText[i];
                var charIndex = EnglishAlphabet.IndexOf(plainTextChar);
                var cipherTextChar = currentAlphabet[charIndex];
                cipherTextBuilder.Append(cipherTextChar);
            }
            return cipherTextBuilder.ToString();
        }

        public static string Decrypt(string[] alphabets, string cipherText)
        {
            var plainTextBuilder = new StringBuilder();
            for (int i = 0; i < cipherText.Length; i++)
            {
                var alphabetNumber = i % alphabets.Length;
                string currentAlphabet = alphabets[alphabetNumber];
                var cipherTextChar = cipherText[i];
                var charIndex = currentAlphabet.IndexOf(cipherTextChar);
                var plainTextChar = EnglishAlphabet[charIndex];
                plainTextBuilder.Append(plainTextChar);
            }
            return plainTextBuilder.ToString();
        }
    }
}
