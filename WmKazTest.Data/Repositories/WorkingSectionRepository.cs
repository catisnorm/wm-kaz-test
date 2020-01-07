using WmKazTest.Data.Model;

namespace WmKazTest.Data.Repositories
{
    public class WorkingSectionRepository : Repository<WorkingSection>
    {
        public WorkingSectionRepository(ObservationDataContext context) : base(context)
        {
        }
    }
}