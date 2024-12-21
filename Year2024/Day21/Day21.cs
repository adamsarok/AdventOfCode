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
			public Point CurrentPos;
			protected Keypad target;
			protected Point startingPosition;

			//public abstract List<Point> Translate(List<Point> input);
			public abstract void Press(List<Point> buttons);
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
		
			public override void Press(List<Point> buttons) {
				List<Point> output = new List<Point>();
				foreach (var p in buttons) {
					if (p.x >= width || p.x < 0 || p.y >= height || p.y < 0) return;
					var button = inputs[p.y, p.x];
					switch (button) {
						case '^':
							target.CurrentPos += new Point(0, -1);
							break;
						case 'v':
							target.CurrentPos += new Point(0, 1);
							break;
						case '<':
							target.CurrentPos += new Point(-1, 0);
							break;
						case '>':
							target.CurrentPos += new Point(1, 0);
							break;
						case 'A': //I know this is terrible OOP but its too early in the morning to think
							target.Press(new List<Point>() { target.CurrentPos });
							break;
						default: //empty button pressed, wrong input
							return;
					}
				}
			}
			public List<Point> Translate(char[] input) {
				var result = new List<Point>();
				//Point pos = startingPos;
				foreach (var c in input) {
					switch (c) {
						case '^':
							result.Add(new Point(1, 0));
							break;
						case 'v':
							result.Add(new Point(1, 1));
							break;
						case '<':
							result.Add(new Point(0, 1));
							break;
						case '>':
							result.Add(new Point(2, 1));
							break;
						case 'A':
							result.Add(new Point(2, 0));
							break;
						default:
							throw new Oopsie("Invalid input");
					}
				}
				return result;
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
			public override void Press(List<Point> buttons) {
				foreach (var p in buttons) {
					//Console.Write(inputs[p.y, p.x]);
					if (p.x >= width || p.x < 0 || p.y >= height || p.y < 0) return;
					Output += inputs[p.y, p.x];
				}
			}
		}
		private void Test() {
			List<TestInput> testinputs = new List<TestInput> {
				new TestInput("029A", "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A"),
				new TestInput("980A", "<v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<v<A>A>^AAAvA<^A>A<vA>^A<A>A"),
				new TestInput("179A", "<v<A>>^A<vA<A>>^AAvAA<^A>A<v<A>>^AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
				new TestInput("456A", "<v<A>>^AA<vA<A>>^AAvAA<^A>A<vA>^A<A>A<vA>^A<A>A<v<A>A>^AAvA<^A>A"),
				new TestInput("379A", "<v<A>>^AvA^A<vA<AA>>^AAvA<^A>AAvA^A<vA>^AA<A>A<v<A>A>^AAAvA<^A>A"),
				new TestInput("0", "<vA<AA>>^AvAA<^A>A"),
			};

			foreach (var i in testinputs) {
				Console.WriteLine(i.expected);
				var t = robot1.Translate(i.input.ToCharArray());
				robot1.Press(t);
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
			
			//Test();

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
