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

                if (string.IsNullOrWhiteSpace(observation.Color) ||
                    !new[] { "green", "red" }.Contains(observation.Color))
                    throw new ArgumentException("Invalid color. Acceptable values: 'green' or 'red'.",
                        nameof(observation.Color));

                if (observation.Numbers.Any(str => str.Length != 7) || observation.Numbers.Any(str =>
                        str.Except(new[] { '0', '1' }).Any()))
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
                    await SetWorkingSections(display, workingSections, existingSequence);
                }

                await UnitOfWork.Save();

                var start = GetPossibleStart(observation.Numbers);
                var missing = GetMissingSections();
                
                existingSequence.PossibleStart = start;
                existingSequence.Missing = missing;
                await UpdateSequence(existingSequence);

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

        private static string[] GetMissingSections()
        {
            return new[] { "0000000", "0000000" };
        }

        private static int[] GetPossibleStart(IEnumerable<string> numbers) // TODO: complete method
        {
            var possibleNumber = new List<int>();
            var arrays = numbers.Select(TruthTable.GetPossibleDigits).ToList();
            var longest = arrays.Max(arr => arr.Count());
            for (var i = 0; i < longest; i++)
            {
                var index = i;
                possibleNumber.Add(int.Parse(string.Join("", arrays.Select(arr => arr.ElementAtOrDefault(index)))));
            }
            return possibleNumber.ToArray();
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