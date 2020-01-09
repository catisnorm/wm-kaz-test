using System;
using System.Collections.Generic;
using System.Linq;

namespace WmKazTest.Core.Utils
{
    public static class TruthTable
    {
        private static string[] ValidNumbers => new[]
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

        private static int GetDigit(string sections)
        {
            return Array.IndexOf(ValidNumbers, sections);
        }

        private static IEnumerable<int> GetPossibleDigits(string sections)
        {
            var workingSections = sections.Select((c, i) => c == '1' ? i : -1).Where(index => index > -1);
            return from number in ValidNumbers
                let withIndex = number.Select((c, i) => new { c, i })
                where withIndex.Count(sec => workingSections.Contains(sec.i) && sec.c == '1') == workingSections.Count()
                select GetDigit(number);
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

        public static IEnumerable<int> GetPossibleNumbers(IEnumerable<string> numbers)
        {
            var arrays = numbers.Select(GetPossibleDigits).ToList();
            return from first in arrays.First()
                from second in arrays.Skip(1).First()
                select int.Parse(first + "" + second);
        }
    }
}