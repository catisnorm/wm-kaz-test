using System;
using System.Threading.Tasks;
using AutoMapper;
using WmKazTest.Core.Model;
using WmKazTest.Data.Interfaces;

namespace WmKazTest.Core.Services
{
    public class SequenceService : ServiceBase
    {
        public SequenceService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Sequence> Create()
        {
            try
            {
                var result = await UnitOfWork.SequenceRepository.Add(new Data.Model.Sequence());
                await UnitOfWork.Save();
                return Mapper.Map<Sequence>(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Clear()
        {
            try
            {
                UnitOfWork.SequenceRepository.DeleteAll();
                await UnitOfWork.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}