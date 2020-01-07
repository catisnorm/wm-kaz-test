using WmKazTest.Data.Model;

namespace WmKazTest.Data.Repositories
{
    public class SequenceRepository : Repository<Sequence>
    {
        public SequenceRepository(ObservationDataContext context) : base(context)
        {
        }
    }
}