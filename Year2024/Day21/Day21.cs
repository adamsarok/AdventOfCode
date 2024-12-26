using Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Keypad = System.Collections.Generic.Dictionary<Helpers.Vec, char>;

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

		State start = new State(new Vec(2, 0), new Vec(2, 0), new Vec(2, 3), "");
		class Machines {

			public Machines(State s) {
				robot1Pos = s.robot1Pos;
				robot2Pos = s.robot2Pos;
				numericKeypadPos = s.numericKeypadPos;
				output = s.output;
			}
			public State State => new State(robot1Pos, robot2Pos, numericKeypadPos, output);
			private static DirectionalInput up = new DirectionalInput(Type.Move, new Vec(0, -1));
			private static DirectionalInput down = new DirectionalInput(Type.Move, new Vec(0, 1));
			private static DirectionalInput left = new DirectionalInput(Type.Move, new Vec(-1, 0));
			private static DirectionalInput right = new DirectionalInput(Type.Move, new Vec(1, 0));
			private static DirectionalInput a = new DirectionalInput(Type.PressA, new Vec(0, 0));
			private static DirectionalInput none = new DirectionalInput(Type.None, new Vec(0, 0));
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
			record DirectionalInput(Type type, Vec vec);
			private static DirectionalInput[,] directionalKeypad = new DirectionalInput[,] {
					{ none, up, a },
					{ left, down, right },
			};
			private DirectionalInput GetDir(Vec p) {
				if (p.x >= 3 || p.x < 0 || p.y >= 2 || p.y < 0) return none;
				return directionalKeypad[p.y, p.x];
			}

			//37 is causing the problem
			//3->7 = can be <AAv<AA>>A
			//or			vv<<AA>^AA>A
			//here we are not finding the ideal cost, but why?

			private static char[,] numericInputs = new char[,] {
					{ '7', '8', '9' },
					{ '4', '5', '6' },
					{ '1', '2', '3' },
					{ ' ', '0', 'A' }
				};
			protected int height, width;
			//private Point robot1Pos;
			private Vec robot1Pos;
			private Vec robot2Pos;
			private Vec numericKeypadPos;
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


		record TestInput(string expected, string input) { }
		record State(Vec robot1Pos, Vec robot2Pos, Vec numericKeypadPos, string output) { }

		private Dictionary<Vec, char> directionalInputs = new Dictionary<Vec, char>() {
			{ new Vec(1, 0), '^' },
			{ new Vec(1, 1), 'v' },
			{ new Vec(0, 0), ' ' },
			{ new Vec(0, 1), '<' },
			{ new Vec(2, 1), '>' },
			{ new Vec(2, 0), 'A' }
		};
		private Dictionary<Vec, char> numericInputs = new Dictionary<Vec, char>() {
			{ new Vec(0, 0), '7' },
			{ new Vec(1, 0), '8' },
			{ new Vec(2, 0), '9' },
			{ new Vec(0, 1), '4' },
			{ new Vec(1, 1), '5' },
			{ new Vec(2, 1), '6' },
			{ new Vec(0, 2), '1' },
			{ new Vec(1, 2), '2' },
			{ new Vec(2, 2), '3' },
			{ new Vec(0, 3), ' ' },
			{ new Vec(1, 3), '0' },
			{ new Vec(2, 3), 'A' },
		};

		ConcurrentDictionary<(char from, char to, int level), long> cache;

		protected override long SolvePart1() {
			return Solve(2);
		}

		private long EncodeKeys(string l, Keypad[] keypads) {
			if (keypads.Length == 0) return l.Length;	
			var currentKey = 'A';
			long result = 0;
			foreach (var k in l) {
				result += EncodeKey(currentKey, k, keypads);
				currentKey = k;
			}
			return result;
		}

		private long EncodeKey(char from, char to, Keypad[] keypads) {
			return cache.GetOrAdd((from, to, keypads.Length), _ => {
				var keypad = keypads[0];
				//always go 1 keypad deeper, try 2 permutations, return if on ' '
				var fromVec = keypad.Single(x => x.Value == from).Key;
				var toVec = keypad.Single(x => x.Value == to).Key;
				var vec = toVec - fromVec;
				string horizontal = "";
				string vertical = "";
				for (int i = 0; i < Math.Abs(vec.y); i++) vertical += vec.y < 0 ? '^' : 'v';
				for (int i = 0; i < Math.Abs(vec.x); i++) horizontal += vec.x < 0 ? '<' : '>';
				var cost = long.MaxValue;
				if (keypad[new Vec(fromVec.x, toVec.y)] != ' ') {
					cost = Math.Min(cost, EncodeKeys($"{vertical}{horizontal}A", keypads[1..]));
				}
				if (keypad[new Vec(toVec.x, fromVec.y)] != ' ') {
					cost = Math.Min(cost, EncodeKeys($"{horizontal}{vertical}A", keypads[1..]));
				}
				return cost;
			});
		}

		private long Solve(int depth) {
			var temp = new List<Keypad> {
				numericInputs
			};
			temp.AddRange(Enumerable.Repeat(directionalInputs, depth));
			var keypads = temp.ToArray();
			cache = new();
			var result = 0L;
			foreach (var l in input) {
				var num = int.Parse(l[..^1]);
				var r = EncodeKeys(l, keypads);
				Console.WriteLine($"{l}:{r}");
				result += num * r;
			}
			return result;
		}

		protected override long SolvePart2() {
			return Solve(25);
		}
	}
}
