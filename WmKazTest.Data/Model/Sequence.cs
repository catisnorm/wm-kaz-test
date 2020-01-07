using System;
using System.Collections.Generic;

namespace WmKazTest.Data.Model
{
    public class Sequence
    {
        public Guid Id { get; set; }
        public int[] PossibleStart { get; set; }
        public string[] Missing { get; set; }

        public ICollection<Observation> Observations { get; set; }
    }
}