using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using WmKazTest.Core.Model;
using WmKazTest.Core.Utils;
using WmKazTest.Data.Interfaces;

namespace WmKazTest.Core.Services
{
    public class ObservationService : ServiceBase
    {
        public ObservationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public async Task<Response> AddObservation(Observation observation)
        {
            try
            {
                #region Validation

                if (string.IsNullOrWhiteSpace(observation.Color) || !observation.Color.Equals("red") ||
                    !observation.Color.Equals("green"))
                    throw new ArgumentException("Invalid color. Acceptable values: 'green' or 'red'.",
                        nameof(observation.Color));

                if (observation.Numbers.Any(str => str.Length != 7) || observation.Numbers.All(str =>
                        !str.Except(new[] { '0', '1' }).Any()))
                    throw new ArgumentException(ErrorMessage.InvalidNumberFormat, nameof(observation.Numbers));

                var existingSequence = await UnitOfWork.SequenceRepository.Get(observation.SequenceId);
                if (existingSequence == null)
                    throw new KeyNotFoundException(ErrorMessage.SequenceNotFound);

                if (!existingSequence.Observations.Any() && observation.Color.Equals("red"))
                    throw new InvalidOperationException(ErrorMessage.NotEnoughData);

                if (existingSequence.Observations.Any(ob => ob.Color.ToLower().Equals("red")))
                    throw new InvalidOperationException(ErrorMessage.RedObservationShouldBeLast);

                if (observation.Numbers.Length < 2 ||
                    existingSequence.Observations.Any(ob => ob.Numbers.SequenceEqual(observation.Numbers)))
                    throw new ArgumentException(ErrorMessage.NoSolutionsFound);

                #endregion

                observation.ReadableValue = TruthTable.GetHumanReadableValue(observation.Numbers);
                await UnitOfWork.ObservationRepository.Add(Mapper.Map<Data.Model.Observation>(observation));

                foreach (var (number, display) in observation.Numbers.Select((num, i) => (num, i)))
                {
                    var workingSections =
                        number.Select((c, section) => c == '1' ? section : -1).Where(index => index > -1);
                    await SetWorkingSections(display, observation.SequenceId, workingSections);
                }

                await UnitOfWork.Save();

                var start = new int[] { };
                var missing = new[] { "0000000", "0000000" };
                if (observation.Numbers.Select(TruthTable.GetDigit).All(i => i > -1))
                {
                    start[0] = TruthTable.GetHumanReadableValue(observation.Numbers);
                    existingSequence.PossibleStart = start;
                    existingSequence.Missing = missing;
                    await UpdateSequence(existingSequence);
                }
                else
                {
                    start = GetPossibleStart();
                    missing = GetMissingSections();
                }

                return new Response
                {
                    Missing = missing,
                    Start = start
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string[] GetMissingSections()
        {
            throw new NotImplementedException();
        }

        private int[] GetPossibleStart()
        {
            throw new NotImplementedException();
        }

        private async Task UpdateSequence(Data.Model.Sequence sequence)
        {
            UnitOfWork.SequenceRepository.Update(sequence);
            await UnitOfWork.Save();
        }

        private async Task SetWorkingSections(int display, Guid sequenceId, IEnumerable<int> workingSections)
        {
            foreach (var section in workingSections)
            {
                if (await SectionWorks(sequenceId, display, section)) continue;
                await UnitOfWork.WorkingSectionRepository.Add(new Data.Model.WorkingSection
                {
                    DisplayIndex = display,
                    Section = section,
                    SequenceId = sequenceId
                });
            }
        }

        private async Task<bool> SectionWorks(Guid sequenceId, int displayIndex, int section)
        {
            return (await UnitOfWork.WorkingSectionRepository.Get(
                ws => ws.SequenceId == sequenceId
                      && ws.DisplayIndex == displayIndex && ws.Section == section
            )).Any();
        }
    }
}