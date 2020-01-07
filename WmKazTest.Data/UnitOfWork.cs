using System.Threading.Tasks;
using WmKazTest.Data.Interfaces;
using WmKazTest.Data.Repositories;

namespace WmKazTest.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ObservationDataContext _context;
        
        public ObservationRepository ObservationRepository { get; }
        public SequenceRepository SequenceRepository { get; }
        public WorkingSectionRepository WorkingSectionRepository { get; }

        public UnitOfWork(ObservationDataContext context)
        {
            _context = context;
            ObservationRepository = new ObservationRepository(_context);
            SequenceRepository = new SequenceRepository(_context);
            WorkingSectionRepository = new WorkingSectionRepository(_context);
        }
        
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}