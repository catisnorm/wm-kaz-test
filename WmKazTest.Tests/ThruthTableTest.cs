using WmKazTest.Core.Utils;
using Xunit;

namespace WmKazTest.Tests
{
    public class ThruthTableTest
    {
        [Theory]
        [InlineData("1111011", "1010101")]
        [InlineData("1111010", "1011101")]
        [InlineData("0000000", "1111110")]
        public void GetHumanReadableValue_ReturnsNegativeIfValueNotInTruthTable(params string[] numbers)
        {
            var result = TruthTable.GetHumanReadableValue(numbers);
            Assert.True(result < 0);
        }
        
        [Theory]
        [InlineData("1110111", "1111011")]
        [InlineData("1111011", "0111010")]
        [InlineData("1111011", "1111011")]
        [InlineData("1010010", "1011011")]
        public void GetHumanReadableValue_ReturnsInRangeIfValueInTruthTable(params string[] numbers)
        {
            var result = TruthTable.GetHumanReadableValue(numbers);
            Assert.InRange(result, 0, 99);
        }
    }
}