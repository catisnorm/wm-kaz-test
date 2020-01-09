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
                    observation.Numbers = new[] { "1110111", "1110111" };
                    observation.ReadableValue = 0;
                    observation.PossibleReadableValues = new[] { 0 };
                }
                else
                {
                    observation.ReadableValue = TruthTable.GetHumanReadableValue(observation.Numbers);
                    observation.PossibleReadableValues = TruthTable.GetPossibleNumbers(observation.Numbers).ToArray();
                }

                await UnitOfWork.ObservationRepository.Add(Mapper.Map<Data.Model.Observation>(observation));

                foreach (var (number, display) in observation.Numbers.Select((num, i) => (num, i)))
                {
                    var workingSections =
                        number.Select((c, section) => c == '1' ? section : -1).Where(index => index > -1);
                    await SetWorkingSections(display, workingSections, existingSequence);
                }

                await UnitOfWork.Save();

                var missing = GetMissingSections(existingSequence.WorkingSections);
                var possibleStart = GetPossibleStart(existingSequence.Observations);
                if (possibleStart.Length == 0)
                    throw new InvalidOperationException(ErrorMessage.NoSolutionsFound);
                existingSequence.PossibleStart = possibleStart;
                existingSequence.Missing = missing;
                UnitOfWork.SequenceRepository.Update(existingSequence);
                await UnitOfWork.Save();

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

        private static int[] GetPossibleStart(IEnumerable<Data.Model.Observation> existingSequenceObservations)
        {
            var numbers = existingSequenceObservations.Select(o => o.PossibleReadableValues).ToList();
            var length = numbers.Count;

            var sequences = new List<int[]>();
            var firstArr = numbers.First();
            foreach (var val in firstArr)
            {
                var localSequence = new List<int>();
                for (var i = 0; i < length; i++)
                {
                    var currentSequence = numbers.ElementAt(i);
                    if (currentSequence.Contains(val - i))
                        localSequence.Add(currentSequence.First(item => item == val - i));
                    else
                        break;
                }

                sequences.Add(localSequence.ToArray());
            }

            return sequences.Where(seq => seq.Length == length).Select(s => s.First()).ToArray();
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