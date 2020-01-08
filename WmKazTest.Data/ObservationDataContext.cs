using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WmKazTest.Data.Model;

namespace WmKazTest.Data
{
    public class ObservationDataContext : DbContext
    {
        public ObservationDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<WorkingSection> WorkingSections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Observation>().Property(observation => observation.Numbers)
                .HasConversion(arr => string.Join(",", arr),
                    s => s.Split(",", StringSplitOptions.RemoveEmptyEntries))
                .Metadata.SetValueComparer(new ValueComparer<string[]>(
                    (s, s1) => s.SequenceEqual(s1),
                    s => s.GetHashCode()));
            modelBuilder.Entity<Observation>().Property(observation => observation.PossibleReadableValues)
                .HasConversion(arr => string.Join(",", arr),
                    s => s.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                .Metadata.SetValueComparer(new ValueComparer<int[]>((ints, ints1) => ints.SequenceEqual(ints1),
                    ints => ints.GetHashCode()));
            modelBuilder.Entity<Observation>().Property(observation => observation.ReadableValue)
                .HasConversion(val => val < 0 ? null : val,
                    val => val);

            modelBuilder.Entity<Sequence>().Property(sequence => sequence.PossibleStart)
                .HasConversion(arr => string.Join(",", arr),
                    dbVal => dbVal.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                .Metadata.SetValueComparer(new ValueComparer<int[]>((ints, ints1) => ints.SequenceEqual(ints1),
                    ints => ints.GetHashCode()));
            modelBuilder.Entity<Sequence>().Property(sequence => sequence.Missing)
                .HasConversion(arr => string.Join(",", arr),
                    dbVal => dbVal.Split(",", StringSplitOptions.RemoveEmptyEntries))
                .Metadata.SetValueComparer(new ValueComparer<string[]>((strs, strs1) => strs.SequenceEqual(strs1),
                    strs => strs.GetHashCode()));
        }
    }
}