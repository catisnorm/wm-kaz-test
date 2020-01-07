using System;

namespace WmKazTest.Core.Model
{
    public class Sequence
    {
        public Guid Id { get; set; }
        public int[] PossibleStart { get; set; }
        public string[] Missing { get; set; }
    }
}