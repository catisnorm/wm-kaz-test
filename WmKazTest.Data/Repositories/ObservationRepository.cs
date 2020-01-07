using WmKazTest.Data.Model;

namespace WmKazTest.Data.Repositories
{
    public class ObservationRepository : Repository<Observation>
    {
        public ObservationRepository(ObservationDataContext context) : base(context)
        {
        }
    }
}