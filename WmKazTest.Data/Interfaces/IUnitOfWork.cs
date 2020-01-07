using System.Threading.Tasks;
using WmKazTest.Data.Repositories;

namespace WmKazTest.Data.Interfaces
{
    public interface IUnitOfWork
    {
        ObservationRepository ObservationRepository { get; }
        SequenceRepository SequenceRepository { get; }
        WorkingSectionRepository WorkingSectionRepository { get; }

        Task Save();
    }
}