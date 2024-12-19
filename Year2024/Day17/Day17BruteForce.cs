using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Year2024.Day17 {
	public class Day17BruteForce : Solver {
		public Day17BruteForce() : base(2024, 17) {
		}
		class Brute(long from, long to) {
			long A, B, C;
			//int[] target = new int[] { 2, 4, 1, 3, 7, 5, 0, 3, 1, 5, 4, 4, 5, 5, 3, 0 };
			//int[] target = new int[] { 7, 5, 5, 3, 0 }; //5104
			int[] target = new int[] { 5, 0 }; //5104
			int actOut;
			private bool HandCoded2() {
				while (A != 0) {
					B = ((A % 8) ^ 3);		//2,4 1,3
					C = A >> (int)B;		//7,5 
					A = A >> 3;				//0,3
					B = B ^ 5;				//1,5
					B = B ^ C;				//4,4
					//output.Add(B % 8);	//5,5
					if (B % 8 != target[actOut]) return false;
					actOut--;
					if (actOut < 0) {
						return A == 0;
					}
				}
				return false;
			}
			public long Solve() {
				for (long a = from; a <= to; a++) {
					A = a;
					B = 0;
					C = 0;
					actOut = target.Length - 1;
					if (HandCoded2()) {
						return a;
					}
				}
				return -1;
			}
		}

		protected override long SolvePart2() {
			//lets take the brute force solution and turn it up to 11
			//1. hand-roll the "cpu"
			//2. return early once the current character of the input is not matching
			//3. start X threads each starting from a reasonable interval
			//4. profit
			//5. should start from 8 ^ 15 = 35184372088832, end at 8 ^ 16

			//oh it would still take 4000+ hours to finish :D

			long max = (long)Math.Pow(8, 16);

			var path = @"C:\Users\sarok\source\repos\AdventOfCode\Year2024\Day17\chunks.txt";

			Brute b = new Brute(0, 100000);
			var r = b.Solve();
			return -1;

			if (!File.Exists(path)) {
				File.WriteAllText(path, ((long)Math.Pow(8, 15) - 1).ToString());
			}

			for (int dum = 0; dum < 100; dum++) {
				Stopwatch sw = Stopwatch.StartNew();

				long start = 0; //File.ReadAllLines(path).Select(l => long.Parse(l)).Max();
				long step = (long)Math.Pow(10, 10);
				long end = start + step * 32;

				List<Task<long>> tasks = new List<Task<long>>();

				for (int i = 0; i < 32; i++) {
					long from = start + i * step;
					long to = from + step;
					Brute brute = new Brute(from, to);
					tasks.Add(Task.Run(() => brute.Solve()));
				}

				Task.WaitAll(tasks.ToArray());

				foreach (var task in tasks) {
					if (task.Result != -1) {
						File.AppendAllText(path, $"{Environment.NewLine} GOTCHA! {task.Result}");
					}
				}
				File.AppendAllText(path, $"{Environment.NewLine}{end}");
				Console.WriteLine($"{start}-{end} done in {sw.ElapsedMilliseconds}");

				if (end >= max) {
					break;
				}
			}
			return -1;
		}

		protected override long SolvePart1() {
			return 0;
		}

		protected override void ReadInputPart1(string fileName) {
			//throw new NotImplementedException();
		}

		protected override void ReadInputPart2(string fileName) {
			//throw new NotImplementedException();
		}
	}
}
