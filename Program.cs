using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SafeCracker
{
	internal class Program
	{
		private const int RING_SIZE = 16;
		private const int TARGET_VALUE = 50;

		private static IEnumerable<IEnumerable<int>> GetCompleteRings()
		{
			//Outermost inwards
			yield return Ring0();
			yield return Ring1();
			yield return Ring2();
			yield return Ring3();
			yield return Ring4();
		}

		private static IEnumerable<int> Ring0()
		{
			yield return 0;
			yield return 16;
			yield return 8;
			yield return 4;
			yield return 15;
			yield return 7;
			yield return 10;
			yield return 1;
			yield return 10;
			yield return 4;
			yield return 5;
			yield return 3;
			yield return 15;
			yield return 16;
			yield return 4;
			yield return 7;
		}

		private static IEnumerable<int> Ring1()
		{
			yield return 13;
			yield return 11;
			yield return 13;
			yield return 10;
			yield return 18;
			yield return 10;
			yield return 10;
			yield return 10;
			yield return 10;
			yield return 15;
			yield return 7;
			yield return 19;
			yield return 18;
			yield return 2;
			yield return 9;
			yield return 27;
		}

		private static IEnumerable<int> Ring2()
		{
			yield return 0;
			yield return 5;
			yield return 8;
			yield return 5;
			yield return 1;
			yield return 24;
			yield return 8;
			yield return 10;
			yield return 20;
			yield return 7;
			yield return 20;
			yield return 12;
			yield return 1;
			yield return 10;
			yield return 12;
			yield return 22;
		}

		private static IEnumerable<int> Ring3()
		{
			yield return 0;
			yield return 5;
			yield return 20;
			yield return 8;
			yield return 19;
			yield return 10;
			yield return 15;
			yield return 20;
			yield return 12;
			yield return 20;
			yield return 13;
			yield return 13;
			yield return 0;
			yield return 22;
			yield return 19;
			yield return 10;
		}

		private static IEnumerable<int> Ring4()
		{
			yield return 1;
			yield return 14;
			yield return 10;
			yield return 17;
			yield return 10;
			yield return 5;
			yield return 6;
			yield return 18;
			yield return 8;
			yield return 17;
			yield return 4;
			yield return 20;
			yield return 4;
			yield return 14;
			yield return 4;
			yield return 5;
		}

		private static IEnumerable<IEnumerable<int?>> GetPartialRings()
		{
			//Outermost inwards
			yield return PartialRing0();
			yield return InsertNulls(PartialRing1(), true);
			yield return InsertNulls(PartialRing2(), false);
			yield return InsertNulls(PartialRing3(), false);
			yield return InsertNulls(PartialRing4(), true);
		}

		private static IEnumerable<int?> InsertNulls(IEnumerable<int> source, bool startWithNull)
		{
			foreach (int number in source)
			{
				if (startWithNull) { yield return null; }
				yield return number;
				if (!startWithNull) { yield return null; }
			}
		}

		private static IEnumerable<int?> PartialRing0()
		{
			for (int i = 0; i < RING_SIZE; i++)
			{
				yield return null;
			}
		}

		private static IEnumerable<int> PartialRing1()
		{
			yield return 9;
			yield return 6;
			yield return 10;
			yield return 8;
			yield return 10;
			yield return 9;
			yield return 8;
			yield return 8;
		}

		private static IEnumerable<int> PartialRing2()
		{
			yield return 10;
			yield return 0;
			yield return 11;
			yield return 8;
			yield return 8;
			yield return 8;
			yield return 10;
			yield return 11;
		}

		private static IEnumerable<int> PartialRing3()
		{
			yield return 11;
			yield return 3;
			yield return 8;
			yield return 10;
			yield return 14;
			yield return 11;
			yield return 8;
			yield return 12;
		}

		private static IEnumerable<int> PartialRing4()
		{
			yield return 6;
			yield return 6;
			yield return 8;
			yield return 8;
			yield return 16;
			yield return 19;
			yield return 8;
			yield return 17;
		}

		private static void Main()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				PrintResult(GetAllPermutations().FirstOrDefault(permutation => AllSumTo(permutation, TARGET_VALUE)));
			}
			catch { }
			stopwatch.Stop();
			Console.WriteLine($"Finished in {stopwatch.Elapsed}");
			Console.ReadKey();
		}

		private static void PrintResult(IEnumerable<IEnumerable<int>> permutation)
		{
			foreach (IEnumerable<int> ring in permutation)
			{
				Console.WriteLine(string.Join("\t", ring));
			}
		}

		private static IEnumerable<IEnumerable<IEnumerable<int>>> GetAllPermutations()
		{
			return GetAllRotations().Select(rotations => CreatePermutation(GetCompleteRings(), GetPartialRings(), rotations));
		}

		private static IEnumerable<IEnumerable<int>> GetAllRotations()
		{
			return GetAllRotationsUpTo(GetCompleteRings().Count() - 1)
				.Select(rotation => Enumerable.Range(0, 1).Concat(rotation));
		}

		private static IEnumerable<IEnumerable<int>> GetAllRotationsUpTo(int count)
		{
			return GetAllRotationsUpTo(Enumerable.Repeat(Enumerable.Empty<int>(), 1), count);
		}

		private static IEnumerable<IEnumerable<int>> GetAllRotationsUpTo(IEnumerable<IEnumerable<int>> rotationsSoFar, int count)
		{
			if (count == 0) { return rotationsSoFar; }
			var newRotations = rotationsSoFar.SelectMany(rotations => Enumerable.Range(0, RING_SIZE).Select(rotation => rotations.Append(rotation)));
			return GetAllRotationsUpTo(newRotations, count - 1);
		}

		private static bool AllSumTo(IEnumerable<IEnumerable<int>> permutation, int targetValue)
		{
			return TransposeTable(permutation).All(col => col.Aggregate(0, (a, b) => a + b) == targetValue);
		}

		private static IEnumerable<IEnumerable<T>> TransposeTable<T>(IEnumerable<IEnumerable<T>> table)
		{
			for (int i = 0; i < GetMaxRingLength(table); i++)
			{
				yield return GetNthValueFromEachRow(table, i);
			}
		}

		private static IEnumerable<T> GetNthValueFromEachRow<T>(IEnumerable<IEnumerable<T>> table, int i)
		{
			foreach (var row in table)
			{
				yield return row.Skip(i).First();
			}
		}

		private static int GetRingLength<T>(IEnumerable<T> ring)
		{
			return ring.Count();
		}

		private static int GetMaxRingLength<T>(IEnumerable<IEnumerable<T>> rings)
		{
			return rings.Select(GetRingLength).Aggregate(0, Math.Max);
		}

		private static IEnumerable<T> RotateRing<T>(IEnumerable<T> ring, int count)
		{
			int skip;
			if (count < 0)
			{
				skip = ring.Count() - count;
			}
			else
			{
				skip = count;
			}
			return ring.Skip(skip).Concat(ring.Take(skip));
		}

		private static IEnumerable<IEnumerable<int>> CreatePermutation(IEnumerable<IEnumerable<int>> rings, IEnumerable<IEnumerable<int?>> partialRings, IEnumerable<int> rotations)
		{
			return rings.Zip(Enumerable.Repeat(0, 1).Concat(rotations), (ring, rotation) => RotateRing(ring, rotation))
				.Zip(
					partialRings.Zip(rotations, (ring, rotation) => RotateRing(ring, rotation)),
					(ring, partialRing) => CollapseRings(ring, partialRing)
				);
		}

		private static IEnumerable<int> CollapseRings(IEnumerable<int> ring, IEnumerable<int?> partialRing)
		{
			return ring.Zip(partialRing, (full, partial) => partial ?? full);
		}
	}
}