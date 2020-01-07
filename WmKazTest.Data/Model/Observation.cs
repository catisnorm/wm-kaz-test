using System;

namespace WmKazTest.Data.Model
{
    public class Observation
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string[] Numbers { get; set; }
        public int? ReadableValue { get; set; }
        public Guid SequenceId { get; set; }
    }
}