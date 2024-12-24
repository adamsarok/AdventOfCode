using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Year2024.Day24 {
	public class Day24 : Solver {
		private IOrderedEnumerable<Gate> zGates;
		private IOrderedEnumerable<Gate> yGates;
		private int maxPow;

		public Day24() : base(2024, 24) {
		}
		class Gate(Dictionary<string, Gate> circuit, 
			string inputGate1, string inputGate2, string name) {
			public string Name => name;
			public Gate? InputGate1 { get => circuit[inputGate1]; }
			public Gate? InputGate2 { get => circuit[inputGate2]; }
			public virtual bool Output { get; set; }
		}
		Dictionary<string, Gate> Circuit { get; set; }
		class StarterWire : Gate {
			public StarterWire(Dictionary<string, Gate> circuit,
				string inputGate1, string inputGate2, string name, bool init) 
				: base(circuit, inputGate1, inputGate2, name) {
				Output = init;
			}
		}
		class AND(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) {
			public override bool Output {
				get => InputGate1.Output && InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}
		class OR(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) { 
			public override bool Output {
				get => InputGate1.Output || InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}
		class XOR(Dictionary<string, Gate> circuit,
			string inputGate1, string inputGate2, string name) : Gate(circuit, inputGate1, inputGate2, name) { 
			public override bool Output {
				get => InputGate1.Output ^ InputGate2.Output;
				set => throw new Oopsie("cant set gate out directly");
			}
		}

		protected override void ReadInputPart1(string fileName) {
			//input = new();
			IsShort = fileName.Contains("short");
			Circuit = new Dictionary<string, Gate>();
			bool starterWires = true;
			foreach (var l in File.ReadAllLines(fileName)) {
				if (starterWires) {
					if (string.IsNullOrWhiteSpace(l)) {
						starterWires = false;
					} else {
						var s = l.Split(':');
						var g = new StarterWire(Circuit, "", "", s[0], s[1].Trim() == "1" ? true : false);
						Circuit.Add(g.Name, g);
					}
				} else {
					var s = l.Split(' ');
					Gate g;
					switch (s[1]) {
						case "AND":
							g = new AND(Circuit, s[0], s[2], s[4]);
							break;
						case "XOR":
							g = new XOR(Circuit, s[0], s[2], s[4]);
							break;
						case "OR":
							g = new OR(Circuit, s[0], s[2], s[4]);
							break;
						default: throw new Oopsie("Unknown gate");
					}
					Circuit.Add(g.Name, g);
				}
			}
			zGates = Circuit
				.Values
				.Where(x => x.Name.StartsWith("z"))
				.OrderBy(x => x.Name);
			yGates = Circuit
				.Values
				.Where(x => x.Name.StartsWith("y"))
				.OrderBy(x => x.Name);
		}

		protected override void ReadInputPart2(string fileName) {
			ReadInputPart1(fileName);
		}

		protected override long SolvePart1() {			
			return GetResult();
		}

		private long GetResult() {
			int pow = 0;
			long result = 0;
			foreach (var g in zGates) {
				//Console.WriteLine($"{g.Name}:{g.Output}");
				if (g.Output) result += (long)Math.Pow(2, pow);
				pow++;
			}
			return result;
		}

		protected override long SolvePart2() {
			long result = 0;
			if (IsShort) return 0;
			maxPow = int.Parse(Circuit
				.Where(x => x.Key.StartsWith("x"))
				.Select(x => x.Key)
				.Max()
				.Substring(1, 2));
			List<bool> binaryList = new List<bool>();
			for (int i = 0; i < Math.Pow(2, maxPow); i++) {
				if (!TryNum(i, 0)) {
					//we get the first wrong gate at 256 in my example

					//full adder should have
					//OR gates are connected to the output of two AND gates

					//XOR gates are connected to the outputs of:
					//two OR or 1 OR, 1 XOR gates

					//AND gates are connected to:
					//2 OR gates or 1 OR and 1 XOR gates
					CheckCircuit();
				}
			}
			return result;
		}

		private void CheckCircuit() {
			List<string> wrongGates = new List<string>();
			foreach (var g in Circuit.Values) {
				if (g is OR) {
					if (g.InputGate1 is not AND || g.InputGate2 is not AND) {
						wrongGates.Add(g.Name);
					}
				} else if (g is AND) {
					bool isOk = false;
					if (g.InputGate1 is StarterWire && g.InputGate2 is StarterWire) isOk = true;
					else if (g.InputGate1 is OR && g.InputGate2 is OR) isOk = true;
					else if (g.InputGate1 is XOR && g.InputGate2 is OR) isOk = true;
					else if (g.InputGate1 is OR && g.InputGate2 is XOR) isOk = true;
					if (!isOk) wrongGates.Add(g.Name);
				} else if (g is XOR) {
					bool isOk = false;
					if (g.InputGate1 is StarterWire && g.InputGate2 is StarterWire) isOk = true;
					else if (g.InputGate1 is OR && g.InputGate2 is OR) isOk = true;
					else if (g.InputGate1 is XOR && g.InputGate2 is OR) isOk = true;
					else if (g.InputGate1 is OR && g.InputGate2 is XOR) isOk = true;
					if (!isOk) wrongGates.Add(g.Name);
				}
 			}
			var r = string.Join(',', wrongGates.OrderBy(x => x));
		}

		private bool TryNum(long x, long y) {
			SetInputGates("x", x);
			SetInputGates("y", y);
			var r = GetResult();
			return r == (x + y);
		}

		private void SetInputGates(string gate, long number) {
			long acc = number;
			int pow = 0;
			while (acc > 0 || pow <= maxPow) {
				Gate g = Circuit[$"{gate}{pow.ToString("00")}"];
				if (acc > 0) {
					if (g is not StarterWire) throw new Oopsie();
					g.Output = (acc & 1) == 1;
					acc >>= 1;
				} else {
					g.Output = false;
				}
				pow++;
				//binaryList.Add((number & 1) == 1);
			}
		}
	}
}
