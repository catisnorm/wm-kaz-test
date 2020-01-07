using System;

namespace WmKazTest.Model
{
    public class AddObservationViewModel
    {
        public Guid Sequence { get; set; }
        public ObservationViewModel Observation { get; set; }
    }
}