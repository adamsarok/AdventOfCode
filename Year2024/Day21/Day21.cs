using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

			//"<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"
			//actually there are not that many valid combinations - we move to different position and press A between 1 and 3 times
			public List<string> GetValidInputs() { //assume we are alwayy starting from A
				switch (robot1Pos) {
					case { x: 0, y: 0 }: return new List<string>(); //empty button
					case { x: 1, y: 0 }: return new List<string>() {
						">A", ">AA", ">AAA",
						"vA", "vAA", "vAAA",
						"v>A", "v>AA", "v>AAA",
						"v<A", "v<AA", "v<AAA",
					};
					case { x: 2, y: 0 }:
						return new List<string>() {
						"<A", "<AA", "<AAA",
						"vA", "vAA", "vAAA",
						"v<A", "v<AA", "v<AAA",
						"v<<A", "v<<AA", "v<<AAA",
					};
					case { x: 0, y: 1 }:
						return new List<string>() {
						">A", ">AA", ">AAA",
						">>A", ">>AA", ">>AAA",
						">>^A", ">>^AA", ">>^AAA",
						">^A", ">^AA", ">^AAA",
					};
					case { x: 1, y: 1 }:
						return new List<string>() {
						"<A", "<AA", "<AAA",
						">A", ">AA", ">AAA",
						"^A", "^AA", "^AAA",
						">^A", ">^AA", ">^AAA",
					};
					case { x: 2, y: 1 }:
						return new List<string>() {
						"<A", "<AA", "<AAA",
						"<<A", "<<AA", "<<AAA",
						"^A", "^AA", "^AAA",
						"<^A", "<^AA", "<^AAA",
					};
				}
				return new List<string>();
			}
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
					if (robot1Pos.x >= 3 || robot1Pos.x < 0 || robot1Pos.y >= 2 || robot1Pos.y < 0) return false;
				} else if (m1.type == Type.PressA) {
						var m2 = GetDir(robot1Pos);
						switch (m2.type) {
							case Type.None:
								return false; //empty button pressed
							case Type.Move:
								robot2Pos += m2.vec;
								if (robot2Pos.x >= 3 || robot2Pos.x < 0 || robot2Pos.y >= 2 || robot2Pos.y < 0) return false;
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

		//029A:
		//17: L
		//29: U
		//49: R U U
		//67: D D D

		//980A:
		//13: U U U 
		//31: L
		//49: D D D 
		//59: R

		//179A: LLU, UU, RR, DDD
		//456A: LLUU, R, R, DD
		//379A: U, LLUU, RR, DDD

		//the input is not covered by what we know from the test input
		//879A -- LUU, L, RR, DDD
		//508A -- LU, DD, UU, RDDD
		//463A -- 
		//593A
		//189A

		//the last A must be an A press so in order to go a direction twice, we have to copy a previous A
		Dictionary<string, string> inputs = new Dictionary<string, string>() {
			{ "U", "<v<A>>^AvA^A" },
			{ "UU" , "<v<A>>^AAvA^A" },
			{ "UUU" , "<v<A>>^AAAvA^A" },
			{ "D" , "<v<A>A>^AvA<^A>" },
			{ "DD" , "<v<A>A>^AAvA<^A>" },
			{ "DDD" , "<v<A>A>^AAAvA<^A>" },
			{ "L" , "<vA<AA>>^AvAA<^A>A" },
			{ "LL" , "<vA<AA>>^AAvAA<^A>A" },
			{ "LLL" , "<vA<AA>>^AAAvAA<^A>A" },
			{ "R" , "<vA>^A<A>A" },
			{ "RR" , "<vA>^AA<A>A" },
			{ "RRR" , "<vA>^AAA<A>A" }
		};
		//ahhhh this is also not enough, since we need a combination of directions to get to a certain number and it is not as simple as concating these...

		private void Test() {
			List<TestInput> testinputs = new List<TestInput> {
				new TestInput("029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"),
				new TestInput("980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"),
				new TestInput("179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
				new TestInput("456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"),
				new TestInput("379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
				//new TestInput("9", RRR),
				//new TestInput("0", "<vA<AA>>^AvAA<^A>A"), //= [ L, press ] //gotten by debugging the test inputs, could write something clever to automate but...
				//new TestInput("3", "<v<A>>^AvA^A"), //= [ U, press ]
				//new TestInput(" ", "<vA>^A<v<A>^A>AAvA"), //= [ R, U, U, press ] should be invalid
				//new TestInput(" ", "<vA>^A<v<A>^A>AvA"), //= [ R, U, press ] should be invalid
				//new TestInput(" ", "<v<A>A>^AAAvA<^A>"), //= [ D, D, D, press ] should be invalid
				//new TestInput(" ", "<v<A>A>^AvA<^A>"), //= [ D press ] should be invalid
				//new TestInput(" ", "<vA>^A<A>A") // [ R, press ] should be invalid
			};

			//029A -> this means on the numeric: [ left , press ], [ up, press ], ???
			//980A -> this means on the numeric: ???
			//179A -> 
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
		protected override long SolvePart1() {
			if (input.Length == 0) return 0;
			long result = 0;

			//numericKeypad = new NumericKeypad();
			//robot3 = new DirectionalKeypad(numericKeypad);
			//robot2 = new DirectionalKeypad(robot3);
			//robot1 = new DirectionalKeypad(robot2);

			//Test();

			//this is not feasible even with memoization
			//if we need 18 button presses for 0, the combinations are immense
			//we can figure out how to get a certain number when all robots are in the starting position
			//however during a run, the robots will not be in a starting position, so we need to figure out a transition between each output?
			//eg. to get 0 is easy
			//however getting 09 vs 19 is different ->
			//when trying to get to 9 all robots are in a different position when coming from 1 vs coming from 0
			//when a numeric output is generated, that means robot1, robot2 and robot3 are in A position, only numeric position is changed
			//we need to figure out a shortest path to get between numeric positions however

			//while (!found && length <= 9) { 
			//	length++;
			//	found = Combinations(length, combinations, "0");
			//}

			var found = Traverse(new Machines(start), null, "A");

			//foreach (var i in inputs) {
			//	found = Combinations(length, combinations, "");
			//}

			return result;
		}

		char[] combinations = new char[] { '^', 'A', '<', 'v', '>' };
		//maybe recursive?
		private bool Traverse(Machines machines, string nextStep, string target) {
			if (nextStep != null) {
				foreach (var c in nextStep) {
					if (!machines.Press(c)) return false;
				}
				for (int i = 0; i < machines.Output.Length; i++) {
					if (machines.Output[i] != target[i]) return false;
					if (i == machines.Output.Length - 1) return true;
				}
			}
			//this is a good stack overflow generator but not a solution. Since we are pressing the same input 4000 times first
			//not sure this is possible without some kind of intelligence, eg. trying inputs that can reasonably get us to the target
			foreach (var c in machines.GetValidInputs()) {
				if (Traverse(new Machines(machines.State), c, target)) return true;
			}
			return false;
		}



		//HashSet<string> outputs = new HashSet<string>();
		//Dictionary<string, Memo> memos = new Dictionary<string, Memo>();

		//record Memo(Point robot1Pos, Point robot2Pos, Point robot3Pos, Point numericKeypadPos, string output);

		//private bool Combinations(int length, char[] values, string target) {
		//	if (length < 1 || values.Length < 1) return false;
		//	char[] input = new char[length];
		//	int combinations = values.Length;
		//	long all = (long)Math.Pow(combinations, length);
		//	for (long i = 0; i < all; i++) {
		//		long temp = i;
		//		for (int pos = 0; pos < length; pos++) {
		//			input[pos] = values[temp % combinations];
		//			temp /= combinations;
		//		}
		//		Memo m;
		//		var s = new string(input);
		//		if (memos.ContainsKey(s)) continue;
		//		if (memos.TryGetValue(s.Substring(0, s.Length - 1), out m)) {
		//			numericKeypad.Reset();
		//			robot1.CurrentPos = m.robot1Pos;
		//			robot2.CurrentPos = m.robot2Pos;
		//			robot3.CurrentPos = m.robot3Pos;
		//			numericKeypad.CurrentPos = m.numericKeypadPos;
		//			var t = robot1.Translate(new char[] { input.Last() });
		//			robot1.Press(t);
		//		} else {
		//			robot1.Reset();
		//			var t = robot1.Translate(input);
		//			robot1.Press(t);
		//			memos.Add(s, new Memo(robot1.CurrentPos, robot2.CurrentPos, robot3.CurrentPos, numericKeypad.CurrentPos, numericKeypad.Output));
		//		}
		//		//Console.WriteLine($"{s} -> {numericKeypad.Output}");
		//		outputs.Add(numericKeypad.Output);
		//		if (numericKeypad.Output == target) return true;
		//	}
		//	return false;
		//}

		protected override long SolvePart2() {
			long result = 0;

			return result;
		}
	}
}
