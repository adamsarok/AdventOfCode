using Helpers;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Year2024.Day21 {
	public class Day21 : Solver {
		private string[] input;

		public Day21() : base(2024, 21) {
		}
		protected override void ReadInputPart1(string fileName) {
			input = File.ReadAllLines(fileName);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		State start = new State(new Point(2, 0), new Point(2, 0), new Point(2, 3), "");
		class Machines {

			public Machines(State s) {
				robot1Pos = s.robot1Pos;
				robot2Pos = s.robot2Pos;
				numericKeypadPos = s.numericKeypadPos;
				output = s.output;
			}
			public State State => new State(robot1Pos, robot2Pos, numericKeypadPos, output);
			private static DirectionalInput up = new DirectionalInput(Type.Move, new Point(0, -1));
			private static DirectionalInput down = new DirectionalInput(Type.Move, new Point(0, 1));
			private static DirectionalInput left = new DirectionalInput(Type.Move, new Point(-1, 0));
			private static DirectionalInput right = new DirectionalInput(Type.Move, new Point(1, 0));
			private static DirectionalInput a = new DirectionalInput(Type.PressA, new Point(0, 0));
			private static DirectionalInput none = new DirectionalInput(Type.None, new Point(0, 0));
			private static Dictionary<char, DirectionalInput> directionalInputs = new Dictionary<char, DirectionalInput>() {
				{ '^', up },
				{ 'v', down },
				{ '<', left },
				{ '>', right },
				{ 'A', a }
			};
			//		{ ' ', '^', 'A' },
			//		{ '<', 'v', '>' },
			//};
			enum Type { None, Move, PressA }
			record DirectionalInput(Type type, Point vec);
			private static DirectionalInput[,] directionalKeypad = new DirectionalInput[,] {
					{ none, up, a },
					{ left, down, right },
			};
			private DirectionalInput GetDir(Point p) {
				if (p.x >= 3 || p.x < 0 || p.y >= 2 || p.y < 0) return none;
				return directionalKeypad[p.y, p.x];
			}
			private static char[,] numericInputs = new char[,] {
					{ '7', '8', '9' },
					{ '4', '5', '6' },
					{ '1', '2', '3' },
					{ ' ', '0', 'A' }
				};
			protected int height, width;
			//private Point robot1Pos;
			private Point robot1Pos;
			private Point robot2Pos;
			private Point numericKeypadPos;
			private string output;
			//private List<char> steps;
			public string Output => output;
			public bool Press(char c) {
				var m1 = directionalInputs[c];
				if (m1.type == Type.Move) {
					robot1Pos += m1.vec;
					if (robot1Pos.x >= 3 || robot1Pos.x < 0 || robot1Pos.y >= 2 || robot1Pos.y < 0) {
						return false;
					}
				} else if (m1.type == Type.PressA) {
					var m2 = GetDir(robot1Pos);
					switch (m2.type) {
						case Type.None:
							return false; //empty button pressed
						case Type.Move:
							robot2Pos += m2.vec;
							if (robot2Pos.x >= 3 || robot2Pos.x < 0 || robot2Pos.y >= 2 || robot2Pos.y < 0) {
								return false;
							}
							break;
						case Type.PressA:
							var m3 = GetDir(robot2Pos);
							switch (m3.type) {
								case Type.None:
									return false; //empty button pressed
								case Type.Move:
									numericKeypadPos += m3.vec;
									if (numericKeypadPos.x >= 3 || numericKeypadPos.x < 0 || numericKeypadPos.y >= 4 || numericKeypadPos.y < 0) {
										return false;
									}
									break;
								case Type.PressA:
									output += numericInputs[numericKeypadPos.y, numericKeypadPos.x];
									break;
							}
							break;
					}
				}
				return true;
			}
		}



		private void Test() {
			List<TestInput> testinputs = new List<TestInput> {

			};

			Machines m = new Machines(start);

			foreach (var i in testinputs) {
				Console.WriteLine(i.expected);
				foreach (var c in i.input) {
					if (!m.Press(c)) throw new Oopsie("Failed for test input something is wrong");
				}
				Console.WriteLine(m.Output);
				m = new Machines(start);
			}
		}
		record TestInput(string expected, string input) { }
		record State(Point robot1Pos, Point robot2Pos, Point numericKeypadPos, string output) { }

		private Dictionary<char, Point> directionalInputs = new Dictionary<char, Point>() {
			{ '^', new Point(1, 0) },
			{ 'v', new Point(1, 1) },
			{ '<', new Point(0, 1) },
			{ '>', new Point(2, 1) },
			{ 'A', new Point(2, 0) }
		};
		private Dictionary<char, Point> numericInputs = new Dictionary<char, Point>() {
			{ '7', new Point(0, 0) },
			{ '8', new Point(1, 0) },
			{ '9', new Point(2, 0) },
			{ '4', new Point(0, 1) },
			{ '5', new Point(1, 1) },
			{ '6', new Point(2, 1) },
			{ '1', new Point(0, 2) },
			{ '2', new Point(1, 2) },
			{ '3', new Point(2, 2) },
			{ '0', new Point(1, 3) },
			{ 'A', new Point(2, 3) },
		};
		//private char[,] directionalInputs = new char[,] {
		//		{ ' ', '^', 'A' },
		//		{ '<', 'v', '>' },
		//};
		//private char[,] numericInputs = new char[,] {
		//		{ '7', '8', '9' },
		//		{ '4', '5', '6' },
		//		{ '1', '2', '3' },
		//		{ ' ', '0', 'A' }
		//};
		private List<List<char>> NumericToDirectional(char from, char to) {
			List<List<char>> result = new List<List<char>>();
			var permutation1 = new List<char>();
			var vec = numericInputs[to] - numericInputs[from];
			for (int i = 0; i < Math.Abs(vec.y); i++) permutation1.Add(vec.y < 0 ? '^' : 'v');
			for (int i = 0; i < Math.Abs(vec.x); i++) permutation1.Add(vec.x < 0 ? '<' : '>');
			if (vec.x != 0 && vec.y != 0) { //generate second permutation, would this solve 379A?
				List<char> permutation2 = new List<char>();
				var first = permutation1.First();
				var firstCnt = permutation1.Where(x => x == first).Count();
				var second = permutation1.Last();
				var secondcnt = permutation1.Where(x => x == second).Count();
				for (int i = 0; i < secondcnt; i++) permutation2.Add(second);
				for (int i = 0; i < firstCnt; i++) permutation2.Add(first);
				permutation2.Add('A');
				if (!CheckIncorrectNumeric(from, permutation2)) result.Add(permutation2);
			}
			permutation1.Add('A');
			if (!CheckIncorrectNumeric(from, permutation1)) result.Add(permutation1);
			return result;
		}

		private bool CheckIncorrectNumeric(char from, List<char> steps) {
			Point pos = numericInputs[from];
			foreach (var c in steps) {
				switch (c) {
					case '^':
						pos += new Point(0, -1);
						if (CheckInvalidPosNumeric(pos)) {
							return true;
						}
						break;
					case 'v':
						pos += new Point(0, 1);
						if (CheckInvalidPosNumeric(pos)) {
							return true;
						}
						break;
					case '<':
						pos += new Point(-1, 0);
						if (CheckInvalidPosNumeric(pos)) {
							return true;
						}
						break;
					case '>':
						pos += new Point(1, 0);
						if (CheckInvalidPosNumeric(pos)) {
							return true;
						}
						break;
				}
			}
			return false;
		}

		private List<List<char>> DirectionalToDirectional(char from, char to) {
			if (from == to) return new List<List<char>> { new List<char> { 'A' } };
			List<List<char>> result = new List<List<char>>();
			var vec = directionalInputs[to] - directionalInputs[from];
			List<char> permutation1 = new List<char>();
			if (vec.y > 0) for (int i = 0; i < Math.Abs(vec.y); i++) permutation1.Add('v');
			if (vec.x > 0) for (int i = 0; i < Math.Abs(vec.x); i++) permutation1.Add('>');
			if (vec.y < 0) for (int i = 0; i < Math.Abs(vec.y); i++) permutation1.Add('^');
			if (vec.x < 0) for (int i = 0; i < Math.Abs(vec.x); i++) permutation1.Add('<');
			if (vec.x != 0 && vec.y != 0) { //generate second permutation, would this solve 379A?
				List<char> permutation2 = new List<char>();
				var first = permutation1.First();
				var firstCnt = permutation1.Where(x => x == first).Count();
				var second = permutation1.Last();
				var secondcnt = permutation1.Where(x => x == second).Count();
				for (int i = 0; i < secondcnt; i++) permutation2.Add(second);
				for (int i = 0; i < firstCnt; i++) permutation2.Add(first);
				permutation2.Add('A');
				if (!CheckIncorrect(from, permutation2)) result.Add(permutation2);
			}
			permutation1.Add('A');
			if (!CheckIncorrect(from, permutation1)) result.Add(permutation1);
			return result;
		}
		Point invalidNumeric = new Point(0, 3);
		Point invalidDirectional = new Point(0, 0);
		private bool CheckIncorrect(char from, List<char> steps) {
			Point pos = directionalInputs[from];
			foreach (var c in steps) {
				switch (c) {
					case '^':
						pos += new Point(0, -1);
						if (CheckInvalidPos(pos)) {
							return true;
						}
						break;
					case 'v':
						pos += new Point(0, 1);
						if (CheckInvalidPos(pos)) {
							return true;
						}
						break;
					case '<':
						pos += new Point(-1, 0);
						if (CheckInvalidPos(pos)) {
							return true;
						}
						break;
					case '>':
						pos += new Point(1, 0);
						if (CheckInvalidPos(pos)) {
							return true;
						}
						break;
				}
			}
			return false;
		}
		private bool CheckInvalidPos(Point pos) {
			return pos == invalidDirectional || pos.x < 0 || pos.y < 0 || pos.x >= 3 || pos.y >= 2;
		}
		private bool CheckInvalidPosNumeric(Point pos) {
			return pos == invalidNumeric || pos.x < 0 || pos.y < 0 || pos.x >= 3 || pos.y >= 4;
		}
		protected override long SolvePart1() {
			if (input.Length == 0) return 0;
			long result = 0;
			int layers = 2;
			foreach (var inp in input) {
				List<char> res = new List<char>();
				char lastNumeric = 'A';
				foreach (var inputChar in inp) {
					List<List<char>> last = NumericToDirectional(lastNumeric, inputChar);
					for (int i = 0; i < layers; i++) {
						List<List<char>> accs = new List<List<char>>();
						foreach (var l in last) {
							accs.AddRange(TranslateLayer(l));
						}
						last = new List<List<char>>(accs);
					}
					lastNumeric = inputChar;
					foreach (var r in last) Console.WriteLine(string.Join("", r));
					res.AddRange(last.OrderByDescending(x => x.Count).First());
					//379A is still even though I am now generating all iterations?
				}
				//crosscheck:
				Console.WriteLine($"{inp} : {res.Count} : {string.Join("", res)}");
				Machines m = new Machines(start);
				for (int i = 0; i < res.Count; i++) { 
					if (!m.Press(res[i])) throw new Oopsie($"Failed at {i} for test input something is wrong");
				}
				Console.WriteLine(m.Output);

				result += long.Parse(inp.Substring(0,3)) * res.Count; 
			}

			return result;
		}

		private List<List<char>> TranslateLayer(List<char> l) {
			List<List<char>> accs = new List<List<char>>() { new List<char>() };
			char lastChar = 'A';
			foreach (var c in l) {
				var p = DirectionalToDirectional(lastChar, c);
				List<List<char>> newAccs = new List<List<char>>();
				foreach (var acc in accs) {
					if (p.Count > 1) {
						var copy = new List<char>(acc);
						copy.AddRange(p[1]);
						newAccs.Add(copy);
					}
					acc.AddRange(p[0]);
				}
				accs.AddRange(newAccs);
				lastChar = c;
			}
			return accs;
		}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
