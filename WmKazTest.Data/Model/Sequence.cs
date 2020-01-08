using System;
using System.Collections.Generic;

namespace WmKazTest.Data.Model
{
    public class Sequence
    {
        public Guid Id { get; set; }
        public int[] PossibleStart { get; set; }
        public string[] Missing { get; set; }

        public virtual ICollection<Observation> Observations { get; set; }
        public virtual ICollection<WorkingSection> WorkingSections { get; set; }
    }
}