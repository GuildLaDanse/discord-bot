using System;
using System.Text;

namespace BotCommon
{
    public static class RandomStringUtils
    {
        public static string Random(int length)
        {
            return Random("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length);
        }
        
        private static string Random(string chars, int length)
        {
            var randomString = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < length; i++)
                randomString.Append(chars[random.Next(chars.Length)]);

            return randomString.ToString();
        }
    }
}