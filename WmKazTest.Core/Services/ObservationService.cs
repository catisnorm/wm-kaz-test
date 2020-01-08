using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                if (string.IsNullOrWhiteSpace(observation.Color) ||
                    !new[] { "green", "red" }.Contains(observation.Color))
                    throw new ArgumentException("Invalid color. Acceptable values: 'green' or 'red'.",
                        nameof(observation.Color));

                if (observation.Color.Equals("green") &&
                    (observation.Numbers.Any(str => str.Length != 7) || observation.Numbers.Any(str =>
                         str.Except(new[] { '0', '1' }).Any())))
                    throw new ArgumentException(ErrorMessage.InvalidNumberFormat, nameof(observation.Numbers));

                var existingSequence = await UnitOfWork.SequenceRepository.Get(observation.SequenceId);
                if (existingSequence == null)
                    throw new KeyNotFoundException(ErrorMessage.SequenceNotFound);

                if (!existingSequence.Observations.Any() && observation.Color.Equals("red"))
                    throw new InvalidOperationException(ErrorMessage.NotEnoughData);

                if (existingSequence.Observations.Any(ob => ob.Color.ToLower().Equals("red")))
                    throw new InvalidOperationException(ErrorMessage.RedObservationShouldBeLast);

                if (observation.Color.Equals("green") &&
                    (observation.Numbers.Length != 2 || existingSequence.Observations.Any(ob =>
                         ob.Numbers.SequenceEqual(observation.Numbers))))
                    throw new ArgumentException(ErrorMessage.NoSolutionsFound);

                #endregion

                if (observation.Color.Equals("red"))
                {
                    observation.ReadableValue = 0;
                    observation.PossibleReadableValues = new[] { 0 };
                    observation.Numbers = new[] { "1110111", "1110111" };
                    await UnitOfWork.ObservationRepository.Add(Mapper.Map<Data.Model.Observation>(observation));
                    await UnitOfWork.Save();

                    var missingSections = existingSequence.Missing;
                    if (existingSequence.Observations.Count - 1 < 10)
                        missingSections[0] = "0000000";
                    return new Response
                    {
                        Missing = missingSections,
                        Start = new[] { existingSequence.Observations.Count - 1 }
                    };
                }

                observation.ReadableValue = TruthTable.GetHumanReadableValue(observation.Numbers);
                observation.PossibleReadableValues = TruthTable.GetPossibleNumbers(observation.Numbers).ToArray();
                await UnitOfWork.ObservationRepository.Add(Mapper.Map<Data.Model.Observation>(observation));

                foreach (var (number, display) in observation.Numbers.Select((num, i) => (num, i)))
                {
                    var workingSections =
                        number.Select((c, section) => c == '1' ? section : -1).Where(index => index > -1);
                    await SetWorkingSections(display, workingSections, existingSequence);
                }

                await UnitOfWork.Save();

                var currentPossibleNumbers = TruthTable.GetPossibleNumbers(observation.Numbers);
                var missing = GetMissingSections(existingSequence.WorkingSections);

                if (existingSequence.Observations.Count <= 1)
                {
                    existingSequence.PossibleStart = currentPossibleNumbers.ToArray();
                }

                existingSequence.Missing = missing;
                await UpdateSequence(existingSequence);

                return new Response
                {
                    Missing = missing,
                    Start = existingSequence.PossibleStart
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string[] GetMissingSections(IEnumerable<Data.Model.WorkingSection> working)
        {
            var all = new[] { 0, 1, 2, 3, 4, 5, 6 };
            var notWorking = working.GroupBy(ws => ws.DisplayIndex)
                .Select(group => all.Except(group.Select(ws => ws.Section)).ToArray());
            var strings = new List<string>();
            foreach (var sections in notWorking)
            {
                var str = new StringBuilder("0000000");
                foreach (var section in sections)
                {
                    str[section] = '1';
                }

                strings.Add(str.ToString());
            }

            return strings.ToArray();
        }

        private async Task UpdateSequence(Data.Model.Sequence sequence)
        {
            UnitOfWork.SequenceRepository.Update(sequence);
            await UnitOfWork.Save();
        }

        private async Task SetWorkingSections(int display, IEnumerable<int> workingSections,
            Data.Model.Sequence existingSquence)
        {
            foreach (var section in workingSections.Except(existingSquence.WorkingSections
                .Where(ws => ws.DisplayIndex == display).Select(ws => ws.Section)))
            {
                await UnitOfWork.WorkingSectionRepository.Add(new Data.Model.WorkingSection
                {
                    DisplayIndex = display,
                    Section = section,
                    SequenceId = existingSquence.Id
                });
            }
        }
    }
}