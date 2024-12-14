using System.Diagnostics;

namespace Helpers {
	public abstract class Solver(int year, int day) {
		public class Oopsie : Exception {
			public Oopsie() : base("Shouldn't happen") { } //the classic shouldn't happen exception
			public Oopsie(string msg) : base($"Shouldn't happen: {msg}") { } //the detailed shouldn't happen exception
		}

		public void Solve() {
			string daystr = day.ToString("00");
			string shortInput = $"Day{daystr}\\{year}shortinput{day}.txt";
			string input = $"Day{daystr}\\{year}input{day}.txt";
			Measure(() => {
				ReadInputPart1(shortInput);
				var r = SolvePart1();
				WriteResult("Part1 short: ", r);
			});
			Measure(() => {
				ReadInputPart1(input);
				var r2 = SolvePart1();
				WriteResult($"Part1 final: ", r2);
			});
			Measure(() => {
				ReadInputPart2(shortInput);
				var r3 = SolvePart2();
				WriteResult($"Part2 short: ", r3);
			});
			Measure(() => {
				ReadInputPart2(input);
				var r4 = SolvePart2();
				WriteResult($"Part2 final: ", r4);
			});
		}
		private void WriteResult(string txt, long result) {
			Console.Write(txt);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(result);
			Console.ResetColor();
		}
		private void Measure(Action x) {
			Stopwatch sw = Stopwatch.StartNew();
			x();
			sw.Stop();
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write($"  in {sw.ElapsedMilliseconds} ms");
			Console.ResetColor();
			Console.WriteLine();
		}
		protected abstract long SolvePart1();
		protected abstract long SolvePart2();
		protected abstract void ReadInputPart1(string fileName);
		protected abstract void ReadInputPart2(string fileName);

	}
}
