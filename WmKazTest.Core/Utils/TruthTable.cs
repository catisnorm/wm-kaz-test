using System;
using System.Collections.Generic;
using System.Linq;

namespace WmKazTest.Core.Utils
{
    public static class TruthTable
    {
        public static string[] ValidNumbers => new[]
        {
            "1110111",
            "0010010",
            "1011101",
            "1011011",
            "0111010",
            "1101011",
            "1101111",
            "1010010",
            "1111111",
            "1111011"
        };

        public static int GetDigit(string sections)
        {
            return Array.IndexOf(ValidNumbers, sections);
        }

        public static int GetHumanReadableValue(IEnumerable<string> numbers)
        {
            try
            {
                return int.Parse(string.Join("", numbers.Select(s => Array.IndexOf(ValidNumbers, s))));
            }
            catch (FormatException)
            {
                return -1;
            }
        }

        public static int[] GetPossibleDigits()
        {
            throw new NotImplementedException();
        }
    }
}