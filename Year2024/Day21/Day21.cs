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

		abstract class Keypad {
			public string Output;
			public void Reset() {
				Output = "";
				CurrentPos = startingPosition;
				if (target != null) target.Reset();
			}
			protected char[,] inputs;
			protected int height, width;
			public virtual Point CurrentPos { get; set; }
			protected Keypad target;
			protected Point startingPosition;
			public abstract void Press(Point button);
		}

		class DirectionalKeypad : Keypad {
			public DirectionalKeypad(Keypad target) {
				inputs = new char[,] {
					{ ' ', '^', 'A' },
					{ '<', 'v', '>' },
				};
				startingPosition = new Point(2, 0); //this is actually the current position on the TARGET
				CurrentPos = startingPosition;
				this.target = target;
				height = inputs.GetLength(0);
				width = inputs.GetLength(1);
			}

			public override void Press(Point p) {

				if (p.x >= width || p.x < 0 || p.y >= height || p.y < 0) return;
				var button = inputs[p.y, p.x];
				switch (button) {
					case '^':
						target.CurrentPos += new Point(0, -1);
						//if target is the numeric keypad, we can bubble up the direction. in the first keypad, we will know which substring input caused the move
						if (target is NumericKeypad) Console.WriteLine("up");
						break;
					case 'v':
						target.CurrentPos += new Point(0, 1);
						if (target is NumericKeypad) Console.WriteLine("down");
						break;
					case '<':
						target.CurrentPos += new Point(-1, 0);
						if (target is NumericKeypad) Console.WriteLine("left");
						break;
					case '>':
						target.CurrentPos += new Point(1, 0);
						if (target is NumericKeypad) Console.WriteLine("right");
						break;
					case 'A': //I know this is terrible OOP but its too early in the morning to think
						target.Press(target.CurrentPos);
						break;
				}
			}

			public void Press(string input) {
				for (int i = 0; i < input.Length; i++) {
					switch (input[i]) {
						case '^':
							Press(new Point(1, 0));
							break;
						case 'v':
							Press(new Point(1, 1));
							break;
						case '<':
							Press(new Point(0, 1));
							break;
						case '>':
							Press(new Point(2, 1));
							break;
						case 'A':
							Press(new Point(2, 0));
							break;
						default:
							throw new Oopsie("Invalid input");
					}
				}
			}
		}

		class NumericKeypad : Keypad {
			public NumericKeypad() {
				inputs = new char[,] {
					{ '7', '8', '9' },
					{ '4', '5', '6' },
					{ '1', '2', '3' },
					{ ' ', '0', 'A' }
				};
				startingPosition = new Point(2, 3); //A
				CurrentPos = startingPosition;
				height = inputs.GetLength(0);
				width = inputs.GetLength(1);
			}
			public override void Press(Point p) {
				if (p.x >= width || p.x < 0 || p.y >= height || p.y < 0) return;
				Output += inputs[p.y, p.x];
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

		//the last A must be an A press so in order to go a direction twice, we have to copy a previous A
		private const string U = "<v<A>>^AvA^A";
		private const string UU = "<v<A>>^AAvA^A";
		private const string UUU = "<v<A>>^AAAvA^A";
		private const string D = "<v<A>A>^AvA<^A>";
		private const string DD = "<v<A>A>^AAvA<^A>";
		private const string DDD = "<v<A>A>^AAAvA<^A>";
		private const string L = "<vA<AA>>^AvAA<^A>A";
		private const string LL = "<vA<AA>>^AAvAA<^A>A";
		private const string LLL = "<vA<AA>>^AAAvAA<^A>A";
		private const string R = "<vA>^A<A>A";
		private const string RR = "<vA>^AA<A>A";
		private const string RRR = "<vA>^AAA<A>A";
		//ahhhh this is also not enough, since we need a combination of directions to get to a certain number and it is not as simple as concating these...

		private void Test() {
			List<TestInput> testinputs = new List<TestInput> {
				//new TestInput("029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"),
				//new TestInput("980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"),
				//new TestInput("179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
				//new TestInput("456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"),
				//new TestInput("379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
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

			foreach (var i in testinputs) {
				Console.WriteLine(i.expected);
				robot1.Press(i.input);
				Console.WriteLine(numericKeypad.Output);
				robot1.Reset();
			}
		}
		record TestInput(string expected, string input) { }
		protected override long SolvePart1() {
			if (input.Length == 0) return 0;
			long result = 0;

			numericKeypad = new NumericKeypad();
			robot3 = new DirectionalKeypad(numericKeypad);
			robot2 = new DirectionalKeypad(robot3);
			robot1 = new DirectionalKeypad(robot2);

			Test();

			int length = 0;
			var combinations = new char[] { '^', 'A', '<', 'v', '>' };
			bool found = false;

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

			//foreach (var o in outputs) {
			//	//Console.WriteLine(o);
			//}

			return result;
		}


		//HashSet<string> outputs = new HashSet<string>();
		//Dictionary<string, Memo> memos = new Dictionary<string, Memo>();
		private NumericKeypad numericKeypad;
		private DirectionalKeypad robot3;
		private DirectionalKeypad robot2;
		private DirectionalKeypad robot1;
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
