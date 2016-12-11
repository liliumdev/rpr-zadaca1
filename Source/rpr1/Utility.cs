using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace rpr1
{
    public static class Utility
    {
        public static List<string> SeparateString(string text, int maximumWidth)
        {
            List<string> separated = new List<string>();

            int index = 0;
            while (true)
            {
                if (text.Length <= maximumWidth)
                {
                    separated.Add(text);
                    break;
                }
                else
                {
                    string substring = text.Substring(index, maximumWidth);
                    separated.Add(substring.Trim());
                    text = text.Substring(maximumWidth);
                }
            }

            return separated;
        }

        public static int NumberOfWords(string text)
        {
            char[] delimiters = new char[] { '\r', '\n', ' ' };
            return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string[] WordsInString(string text)
        {
            char[] delimiters = new char[] { '\r', '\n', ' ' };
            return text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
