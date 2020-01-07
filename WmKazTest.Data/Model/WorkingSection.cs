using System;
using System.ComponentModel.DataAnnotations;

namespace WmKazTest.Data.Model
{
    public class WorkingSection
    {
        public int Id { get; set; }
        [Range(0, int.MaxValue)]
        public int DisplayIndex { get; set; }
        [Range(0, 6)]
        public int Section { get; set; }
        public Guid SequenceId { get; set; }
    }
}