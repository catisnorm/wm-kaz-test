using AutoMapper;
using WmKazTest.Data.Interfaces;

namespace WmKazTest.Core.Services
{
    public abstract class ServiceBase
    {
        protected IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        protected ServiceBase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }
    }
}